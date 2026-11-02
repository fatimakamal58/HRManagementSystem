using HRManagementSystem.Application.DTOs.LeaveTypes;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities.HR;
using HRManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Application.Services;

public class LeaveTypeService : ILeaveTypeService
{
    private readonly ApplicationDbContext _context;

    public LeaveTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(CreateLeaveTypeDto dto)
    {
        var nameAr = dto.NameAr.Trim();
        var nameEn = dto.NameEn.Trim();

        var exists = await _context.LeaveTypes
            .AnyAsync(x =>
                x.NameAr == nameAr ||
                x.NameEn == nameEn);

        if (exists)
            throw new InvalidOperationException(
                "A leave type with the same name already exists.");

        if (dto.DefaultDaysPerYear.HasValue &&
            dto.DefaultDaysPerYear.Value < 0)
        {
            throw new InvalidOperationException(
                "Default days per year cannot be negative.");
        }

        if (dto.MaxCarryForwardDays.HasValue &&
            dto.MaxCarryForwardDays.Value < 0)
        {
            throw new InvalidOperationException(
                "Maximum carry forward days cannot be negative.");
        }

        if (!dto.AllowCarryForward)
        {
            dto.MaxCarryForwardDays = null;
        }

        var leaveType = new LeaveType
        {
            NameAr = nameAr,
            NameEn = nameEn,
            RequiresBalance = dto.RequiresBalance,
            IncludeNonWorkingDays = dto.IncludeNonWorkingDays,
            IncludePublicHolidays = dto.IncludePublicHolidays,
            DefaultDaysPerYear = dto.DefaultDaysPerYear,
            AllowCarryForward = dto.AllowCarryForward,
            MaxCarryForwardDays = dto.MaxCarryForwardDays,
            IsActive = true
        };

        _context.LeaveTypes.Add(leaveType);

        await _context.SaveChangesAsync();
    }

    public async Task<List<LeaveTypeDto>> GetAllAsync()
    {
        return await _context.LeaveTypes
            .AsNoTracking()
            .OrderBy(x => x.NameAr)
            .Select(x => new LeaveTypeDto
            {
                Id = x.Id,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                RequiresBalance = x.RequiresBalance,
                DefaultDaysPerYear = x.DefaultDaysPerYear,
                AllowCarryForward = x.AllowCarryForward,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<LeaveTypeDto?> GetByIdAsync(int id)
    {
        return await _context.LeaveTypes
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new LeaveTypeDto
            {
                Id = x.Id,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                RequiresBalance = x.RequiresBalance,
                DefaultDaysPerYear = x.DefaultDaysPerYear,
                AllowCarryForward = x.AllowCarryForward,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddApprovalStepAsync(
    CreateLeaveApprovalStepDto dto)
    {
        var leaveTypeExists = await _context.LeaveTypes
            .AnyAsync(x =>
                x.Id == dto.LeaveTypeId &&
                x.IsActive);

        if (!leaveTypeExists)
            throw new InvalidOperationException(
                "Leave type was not found or is inactive.");

        var approverAlreadyExists =
            await _context.LeaveApprovalSteps
                .AnyAsync(x =>
                    x.LeaveTypeId == dto.LeaveTypeId &&
                    x.ApproverType == dto.ApproverType &&
                    x.IsActive);

        if (approverAlreadyExists)
            throw new InvalidOperationException(
                "This approver type already exists in the workflow.");

        var lastStepOrder = await _context.LeaveApprovalSteps
            .Where(x =>
                x.LeaveTypeId == dto.LeaveTypeId &&
                x.IsActive)
            .Select(x => (int?)x.StepOrder)
            .MaxAsync() ?? 0;

        var approvalStep = new LeaveApprovalStep
        {
            LeaveTypeId = dto.LeaveTypeId,
            ApproverType = dto.ApproverType,
            StepOrder = lastStepOrder + 1,
            IsActive = true
        };

        _context.LeaveApprovalSteps.Add(approvalStep);

        await _context.SaveChangesAsync();
    }

    public async Task<List<LeaveApprovalStepDto>> GetApprovalStepsAsync(
    int leaveTypeId)
    {
        return await _context.LeaveApprovalSteps
            .AsNoTracking()
            .Where(x =>
                x.LeaveTypeId == leaveTypeId &&
                x.IsActive)
            .OrderBy(x => x.StepOrder)
            .Select(x => new LeaveApprovalStepDto
            {
                Id = x.Id,
                StepOrder = x.StepOrder,
                ApproverType = x.ApproverType
            })
            .ToListAsync();
    }
}