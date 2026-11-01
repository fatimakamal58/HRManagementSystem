using HRManagementSystem.Application.DTOs.LeaveRequests;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities.HR;
using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Infrastructure.Data;
using HRManagementSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Application.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ApplicationDbContext _context;
    private readonly IWorkCalendarService _workCalendarService;
    private readonly IUserContextService _userContextService;

    public LeaveRequestService(
        ApplicationDbContext context,
        IWorkCalendarService workCalendarService,
        IUserContextService userContextService)
    {
        _context = context;
        _workCalendarService = workCalendarService;
        _userContextService = userContextService;
    }

    public async Task CreateAsync(CreateLeaveRequestDto dto)
    {
        var employeeExists = await _context.Employees
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == dto.EmployeeId &&
                x.IsActive);

        if (!employeeExists)
        {
            throw new InvalidOperationException(
                "Employee does not exist or is inactive.");
        }

        var leaveType = await _context.LeaveTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == dto.LeaveTypeId &&
                x.IsActive);

        if (leaveType is null)
        {
            throw new InvalidOperationException(
                "Leave type does not exist or is inactive.");
        }

        var startDate = dto.StartDate.Date;
        var endDate = dto.EndDate.Date;

        if (endDate < startDate)
        {
            throw new ArgumentException(
                "End date cannot be before start date.");
        }

        if (startDate.Year != endDate.Year)
        {
            throw new InvalidOperationException(
                "A leave request cannot span multiple calendar years.");
        }

        var hasOverlappingLeave = await _context.LeaveRequests
    .AsNoTracking()
    .AnyAsync(x =>
        x.EmployeeId == dto.EmployeeId &&
        (x.Status == LeaveRequestStatus.Pending ||
         x.Status == LeaveRequestStatus.Approved) &&
        x.StartDate <= endDate &&
        x.EndDate >= startDate);

        if (hasOverlappingLeave)
        {
            throw new InvalidOperationException(
                "Employee already has a leave request overlapping this period.");
        }

        var chargedDays = await _workCalendarService.CalculateLeaveDaysAsync(
                            startDate,
                            endDate,
                            leaveType.IncludeNonWorkingDays,
                            leaveType.IncludePublicHolidays
                            );
        if (chargedDays <= 0)
        {
            throw new InvalidOperationException(
                "The selected leave period does not contain any chargeable days.");
        }

        if (leaveType.RequiresBalance)
        {
            var balance = await _context.LeaveBalances
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId == dto.EmployeeId &&
                    x.LeaveTypeId == dto.LeaveTypeId &&
                    x.Year == startDate.Year);

            if (balance is null)
            {
                throw new InvalidOperationException(
                    "Leave balance is not configured for this employee.");
            }

            var total = balance.AllocatedDays + balance.CarryForwardDays;

            var used = await _context.LeaveRequests
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeId == dto.EmployeeId &&
                    x.LeaveTypeId == dto.LeaveTypeId &&
                    x.StartDate.Year == startDate.Year &&
                    x.Status == LeaveRequestStatus.Approved)
                .SumAsync(x => x.ChargedDays);

            var reserved = await _context.LeaveRequests
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeId == dto.EmployeeId &&
                    x.LeaveTypeId == dto.LeaveTypeId &&
                    x.StartDate.Year == startDate.Year &&
                    x.Status == LeaveRequestStatus.Pending)
                .SumAsync(x => x.ChargedDays);

            var available = total - used - reserved;

            if (chargedDays > available)
            {
                throw new InvalidOperationException(
                    $"Insufficient leave balance. Available: {available} days.");
            }
        }

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDate = startDate,
            EndDate = endDate,
            ChargedDays = chargedDays,
            Reason = dto.Reason?.Trim(),
            Status = LeaveRequestStatus.Pending
        };

        var approvalSteps = await _context.LeaveApprovalSteps
                            .AsNoTracking()
                            .Where(x =>
                                x.LeaveTypeId == dto.LeaveTypeId &&
                                x.IsActive)
                            .OrderBy(x => x.StepOrder)
                            .ToListAsync();
        if (approvalSteps.Count == 0)
        {
            throw new InvalidOperationException(
                "Approval workflow is not configured for this leave type.");
        }

        foreach (var step in approvalSteps)
        {
            leaveRequest.Approvals.Add(new LeaveApproval
            {
                StepOrder = step.StepOrder,
                ApproverType = step.ApproverType,
                Status = ApprovalStatus.Pending
            });
        }

        _context.LeaveRequests.Add(leaveRequest);

        await _context.SaveChangesAsync();

    }

    public async Task CancelAsync(int leaveRequestId)
    {
        var leaveRequest = await _context.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == leaveRequestId);

        if (leaveRequest is null)
        {
            throw new InvalidOperationException(
                "Leave request was not found.");
        }

        if (leaveRequest.Status == LeaveRequestStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Leave request is already cancelled.");
        }

        if (leaveRequest.Status == LeaveRequestStatus.Rejected)
        {
            throw new InvalidOperationException(
                "A rejected leave request cannot be cancelled.");
        }

        if (leaveRequest.Status == LeaveRequestStatus.Approved &&
            leaveRequest.StartDate.Date <= DateTime.Today)
        {
            throw new InvalidOperationException(
                "An approved leave request cannot be cancelled after the leave has started.");
        }

        leaveRequest.Status = LeaveRequestStatus.Cancelled;

        await _context.SaveChangesAsync();
    }

    public async Task ApproveAsync(
    int leaveRequestId,
    string approverUserId,
    string? comment)
    {
        var leaveRequest = await _context.LeaveRequests
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(x => x.Id == leaveRequestId);

        if (leaveRequest is null)
        {
            throw new InvalidOperationException(
                "Leave request was not found.");
        }

        if (leaveRequest.Status != LeaveRequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending leave requests can be approved.");
        }

        var currentStep = leaveRequest.Approvals
                            .Where(x => x.Status == ApprovalStatus.Pending)
                            .OrderBy(x => x.StepOrder)
                            .FirstOrDefault();
        if (currentStep is null)
        {
            throw new InvalidOperationException(
                "No pending approval step was found.");
        }

        await ValidateApproverAsync(
            leaveRequest,
            currentStep,
            approverUserId);

        currentStep.Status = ApprovalStatus.Approved;
        currentStep.ApproverUserId = approverUserId;
        currentStep.ActionAt = DateTime.UtcNow;
        currentStep.Comment = comment?.Trim();
       
        var hasPendingSteps = leaveRequest.Approvals
    .Any(x => x.Status == ApprovalStatus.Pending);

        if (!hasPendingSteps)
        {
            leaveRequest.Status = LeaveRequestStatus.Approved;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RejectAsync(
    int leaveRequestId,
    string approverUserId,
    string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException(
                "A rejection comment is required.");

        var leaveRequest = await _context.LeaveRequests
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(x => x.Id == leaveRequestId);

        if (leaveRequest is null)
            throw new InvalidOperationException(
                "Leave request was not found.");

        if (leaveRequest.Status != LeaveRequestStatus.Pending)
            throw new InvalidOperationException(
                "Only pending leave requests can be rejected.");

        var currentStep = leaveRequest.Approvals
            .Where(x => x.Status == ApprovalStatus.Pending)
            .OrderBy(x => x.StepOrder)
            .FirstOrDefault();

        if (currentStep is null)
            throw new InvalidOperationException(
                "No pending approval step was found.");

        await ValidateApproverAsync(
            leaveRequest,
            currentStep,
            approverUserId);

        currentStep.Status = ApprovalStatus.Rejected;
        currentStep.ApproverUserId = approverUserId;
        currentStep.ActionAt = DateTime.UtcNow;
        currentStep.Comment = comment.Trim();

        leaveRequest.Status = LeaveRequestStatus.Rejected;

        await _context.SaveChangesAsync();
    }


    private async Task ValidateApproverAsync(
    LeaveRequest leaveRequest,
    LeaveApproval currentStep,
    string approverUserId)
    {
        if (currentStep.ApproverType == ApproverType.DirectManager)
        {
            var approverEmployeeId =
                await _userContextService.GetEmployeeIdAsync(approverUserId);

            if (approverEmployeeId is null)
                throw new InvalidOperationException(
                    "The current user is not linked to an employee.");

            var employeeManagerId = await _context.Employees
                .AsNoTracking()
                .Where(x => x.Id == leaveRequest.EmployeeId)
                .Select(x => x.ManagerId)
                .FirstOrDefaultAsync();

            if (employeeManagerId != approverEmployeeId)
                throw new UnauthorizedAccessException(
                    "Only the employee's direct manager can process this request.");
        }
        else if (currentStep.ApproverType == ApproverType.HR)
        {
            var isHr = await _userContextService
                .IsInRoleAsync(approverUserId, "HR");

            if (!isHr)
                throw new UnauthorizedAccessException(
                    "Only HR can process this step.");
        }
        else if (currentStep.ApproverType == ApproverType.HRManager)
        {
            var isHrManager = await _userContextService
                .IsInRoleAsync(approverUserId, "HRManager");

            if (!isHrManager)
                throw new UnauthorizedAccessException(
                    "Only the HR Manager can process this step.");
        }
        else
        {
            throw new InvalidOperationException(
                "Unsupported approver type.");
        }
    }
}