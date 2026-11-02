using HRManagementSystem.Application.DTOs.LeaveBalances;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities.HR;
using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Infrastructure.Data;
using HRManagementSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Application.Services;

public class LeaveBalanceService : ILeaveBalanceService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserContextService _userContextService;

    public LeaveBalanceService(
    ApplicationDbContext context,
    IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }

    public async Task SetBalanceAsync(SetLeaveBalanceDto dto)
    {
        var employeeExists = await _context.Employees
            .AnyAsync(x =>
                x.Id == dto.EmployeeId &&
                x.IsActive);

        if (!employeeExists)
            throw new InvalidOperationException(
                "Employee was not found or is inactive.");

        var leaveType = await _context.LeaveTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == dto.LeaveTypeId &&
                x.IsActive);

        if (leaveType is null)
            throw new InvalidOperationException(
                "Leave type was not found or is inactive.");

        if (!leaveType.RequiresBalance)
            throw new InvalidOperationException(
                "This leave type does not require a balance.");

        if (dto.AllocatedDays < 0)
            throw new InvalidOperationException(
                "Allocated days cannot be negative.");

        if (dto.CarryForwardDays < 0)
            throw new InvalidOperationException(
                "Carry forward days cannot be negative.");

        if (!IsValidHalfDayValue(dto.AllocatedDays))
            throw new InvalidOperationException(
                "Allocated days must be in whole or half-day increments.");

        if (!IsValidHalfDayValue(dto.CarryForwardDays))
            throw new InvalidOperationException(
                "Carry forward days must be in whole or half-day increments.");

        if (!leaveType.AllowCarryForward &&
            dto.CarryForwardDays > 0)
        {
            throw new InvalidOperationException(
                "This leave type does not allow carry forward.");
        }

        if (leaveType.AllowCarryForward &&
            leaveType.MaxCarryForwardDays.HasValue &&
            dto.CarryForwardDays > leaveType.MaxCarryForwardDays.Value)
        {
            throw new InvalidOperationException(
                $"Carry forward days cannot exceed {leaveType.MaxCarryForwardDays.Value}.");
        }

        var balance = await _context.LeaveBalances
            .FirstOrDefaultAsync(x =>
                x.EmployeeId == dto.EmployeeId &&
                x.LeaveTypeId == dto.LeaveTypeId &&
                x.Year == dto.Year);

        if (balance is null)
        {
            balance = new LeaveBalance
            {
                EmployeeId = dto.EmployeeId,
                LeaveTypeId = dto.LeaveTypeId,
                Year = dto.Year,
                AllocatedDays = dto.AllocatedDays,
                CarryForwardDays = dto.CarryForwardDays,
                IsActive = true
            };

            _context.LeaveBalances.Add(balance);
        }
        else
        {
            balance.AllocatedDays = dto.AllocatedDays;
            balance.CarryForwardDays = dto.CarryForwardDays;
            balance.IsActive = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<LeaveBalanceDto>> GetAllAsync()
    {
        var balances = await _context.LeaveBalances
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => new
            {
                x.Id,
                x.EmployeeId,
                EmployeeName = x.Employee.FirstNameEn + " " + x.Employee.LastNameEn,
                x.LeaveTypeId,
                LeaveTypeName = x.LeaveType.NameAr,
                x.Year,
                x.AllocatedDays,
                x.CarryForwardDays
            })
            .OrderBy(x => x.EmployeeName)
            .ThenBy(x => x.LeaveTypeName)
            .ThenByDescending(x => x.Year)
            .ToListAsync();

        var leaveUsage = await _context.LeaveRequests
            .AsNoTracking()
            .Where(x =>
                x.Status == LeaveRequestStatus.Approved ||
                x.Status == LeaveRequestStatus.Pending)
            .GroupBy(x => new
            {
                x.EmployeeId,
                x.LeaveTypeId,
                Year = x.StartDate.Year
            })
            .Select(g => new
            {
                g.Key.EmployeeId,
                g.Key.LeaveTypeId,
                g.Key.Year,

                UsedDays = g
                    .Where(x =>
                        x.Status == LeaveRequestStatus.Approved)
                    .Sum(x => x.ChargedDays),

                ReservedDays = g
                    .Where(x =>
                        x.Status == LeaveRequestStatus.Pending)
                    .Sum(x => x.ChargedDays)
            })
            .ToListAsync();

        var result = balances.Select(balance =>
        {
            var usage = leaveUsage.FirstOrDefault(x =>
                x.EmployeeId == balance.EmployeeId &&
                x.LeaveTypeId == balance.LeaveTypeId &&
                x.Year == balance.Year);

            var usedDays = usage?.UsedDays ?? 0;
            var reservedDays = usage?.ReservedDays ?? 0;

            var totalDays =
                balance.AllocatedDays +
                balance.CarryForwardDays;

            return new LeaveBalanceDto
            {
                Id = balance.Id,

                EmployeeId = balance.EmployeeId,
                EmployeeName = balance.EmployeeName,

                LeaveTypeId = balance.LeaveTypeId,
                LeaveTypeName = balance.LeaveTypeName,

                Year = balance.Year,

                AllocatedDays = balance.AllocatedDays,
                CarryForwardDays = balance.CarryForwardDays,

                TotalDays = totalDays,

                UsedDays = usedDays,
                ReservedDays = reservedDays,

                RemainingDays =
                    totalDays - usedDays,

                AvailableDays =
                    totalDays - usedDays - reservedDays
            };
        }).ToList();

        return result;
    }

    public async Task InitializeYearBalancesAsync(
     InitializeLeaveBalanceDto dto)
    {
        if (dto.Year < 2000 || dto.Year > 2100)
            throw new InvalidOperationException(
                "Invalid year.");

        if (dto.LeaveTypes.Count == 0)
            throw new InvalidOperationException(
                "No leave types were selected.");

        if (dto.LeaveTypes.Any(x => x.Days < 0))
            throw new InvalidOperationException(
                "Leave days cannot be negative.");

        if (dto.LeaveTypes.Any(x => !IsValidHalfDayValue(x.Days)))
            throw new InvalidOperationException(
                "Leave days must be in whole or half-day increments.");

        var employeeIds = await _context.Employees
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => x.Id)
            .ToListAsync();

        if (employeeIds.Count == 0)
            throw new InvalidOperationException(
                "No active employees were found.");

        var requestedLeaveTypeIds = dto.LeaveTypes
            .Select(x => x.LeaveTypeId)
            .Distinct()
            .ToList();

        var validLeaveTypeIds = await _context.LeaveTypes
            .AsNoTracking()
            .Where(x =>
                requestedLeaveTypeIds.Contains(x.Id) &&
                x.IsActive &&
                x.RequiresBalance)
            .Select(x => x.Id)
            .ToListAsync();

        if (validLeaveTypeIds.Count != requestedLeaveTypeIds.Count)
            throw new InvalidOperationException(
                "One or more leave types are invalid.");

        var existingBalances = await _context.LeaveBalances
            .AsNoTracking()
            .Where(x =>
                x.Year == dto.Year &&
                requestedLeaveTypeIds.Contains(x.LeaveTypeId))
            .Select(x => new
            {
                x.EmployeeId,
                x.LeaveTypeId
            })
            .ToListAsync();

        var newBalances = new List<LeaveBalance>();

        foreach (var employeeId in employeeIds)
        {
            foreach (var leaveType in dto.LeaveTypes)
            {
                var alreadyExists = existingBalances.Any(x =>
                    x.EmployeeId == employeeId &&
                    x.LeaveTypeId == leaveType.LeaveTypeId);

                if (alreadyExists)
                    continue;

                newBalances.Add(new LeaveBalance
                {
                    EmployeeId = employeeId,
                    LeaveTypeId = leaveType.LeaveTypeId,
                    Year = dto.Year,
                    AllocatedDays = leaveType.Days,
                    CarryForwardDays = 0,
                    IsActive = true
                });
            }
        }

        if (newBalances.Count == 0)
            return;

        _context.LeaveBalances.AddRange(newBalances);

        await _context.SaveChangesAsync();
    }

    public async Task<InitializeLeaveBalanceDto> GetInitializationFormAsync(
    int year)
    {
        if (year < 2000 || year > 2100)
            throw new InvalidOperationException(
                "Invalid year.");

        var leaveTypes = await _context.LeaveTypes
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                x.RequiresBalance &&
                x.DefaultDaysPerYear.HasValue)
            .OrderBy(x => x.NameAr)
            .Select(x => new LeaveTypeInitializationDto
            {
                LeaveTypeId = x.Id,
                LeaveTypeName = x.NameAr,
                Days = x.DefaultDaysPerYear!.Value
            })
            .ToListAsync();

        return new InitializeLeaveBalanceDto
        {
            Year = year,
            LeaveTypes = leaveTypes
        };
    }

    public async Task<UpdateLeaveBalanceDto?> GetForUpdateAsync(int id)
    {
        return await _context.LeaveBalances
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new UpdateLeaveBalanceDto
            {
                Id = x.Id,
                EmployeeName =
                    x.Employee.FirstNameEn + " " + x.Employee.LastNameEn,
                LeaveTypeName = x.LeaveType.NameAr,
                Year = x.Year,
                AllocatedDays = x.AllocatedDays,
                CarryForwardDays = x.CarryForwardDays
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(UpdateLeaveBalanceDto dto)
    {
        var balance = await _context.LeaveBalances
            .Include(x => x.LeaveType)
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (balance is null)
            throw new InvalidOperationException(
                "Leave balance was not found.");

        if (!balance.LeaveType.IsActive)
            throw new InvalidOperationException(
                "Leave type is inactive.");

        if (!balance.LeaveType.RequiresBalance)
            throw new InvalidOperationException(
                "This leave type does not require a balance.");

        if (dto.AllocatedDays < 0)
            throw new InvalidOperationException(
                "Allocated days cannot be negative.");

        if (dto.CarryForwardDays < 0)
            throw new InvalidOperationException(
                "Carry forward days cannot be negative.");

        if (!IsValidHalfDayValue(dto.AllocatedDays))
            throw new InvalidOperationException(
                "Allocated days must be in whole or half-day increments.");

        if (!IsValidHalfDayValue(dto.CarryForwardDays))
            throw new InvalidOperationException(
                "Carry forward days must be in whole or half-day increments.");

        if (!balance.LeaveType.AllowCarryForward &&
            dto.CarryForwardDays > 0)
        {
            throw new InvalidOperationException(
                "This leave type does not allow carry forward.");
        }

        if (balance.LeaveType.AllowCarryForward &&
            balance.LeaveType.MaxCarryForwardDays.HasValue &&
            dto.CarryForwardDays >
            balance.LeaveType.MaxCarryForwardDays.Value)
        {
            throw new InvalidOperationException(
                $"Carry forward days cannot exceed " +
                $"{balance.LeaveType.MaxCarryForwardDays.Value}.");
        }

        balance.AllocatedDays = dto.AllocatedDays;
        balance.CarryForwardDays = dto.CarryForwardDays;

        await _context.SaveChangesAsync();
    }

    public async Task<List<LeaveBalanceDto>> GetMyBalancesAsync(
    string currentUserId)
    {
        var employeeId =
            await _userContextService.GetEmployeeIdAsync(currentUserId);

        if (employeeId is null)
            throw new UnauthorizedAccessException(
                "The current user is not linked to an employee.");

        var balances = await _context.LeaveBalances
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                x.EmployeeId == employeeId.Value)
            .Select(x => new
            {
                x.Id,
                x.EmployeeId,
                EmployeeName =
                    x.Employee.FirstNameEn + " " + x.Employee.LastNameEn,
                x.LeaveTypeId,
                LeaveTypeName = x.LeaveType.NameAr,
                x.Year,
                x.AllocatedDays,
                x.CarryForwardDays
            })
            .OrderByDescending(x => x.Year)
            .ThenBy(x => x.LeaveTypeName)
            .ToListAsync();

        var leaveUsage = await _context.LeaveRequests
            .AsNoTracking()
            .Where(x =>
                x.EmployeeId == employeeId.Value &&
                (x.Status == LeaveRequestStatus.Approved ||
                 x.Status == LeaveRequestStatus.Pending))
            .GroupBy(x => new
            {
                x.LeaveTypeId,
                Year = x.StartDate.Year
            })
            .Select(g => new
            {
                g.Key.LeaveTypeId,
                g.Key.Year,

                UsedDays = g
                    .Where(x =>
                        x.Status == LeaveRequestStatus.Approved)
                    .Sum(x => x.ChargedDays),

                ReservedDays = g
                    .Where(x =>
                        x.Status == LeaveRequestStatus.Pending)
                    .Sum(x => x.ChargedDays)
            })
            .ToListAsync();

        return balances.Select(balance =>
        {
            var usage = leaveUsage.FirstOrDefault(x =>
                x.LeaveTypeId == balance.LeaveTypeId &&
                x.Year == balance.Year);

            var usedDays = usage?.UsedDays ?? 0;
            var reservedDays = usage?.ReservedDays ?? 0;

            var totalDays =
                balance.AllocatedDays +
                balance.CarryForwardDays;

            return new LeaveBalanceDto
            {
                Id = balance.Id,
                EmployeeId = balance.EmployeeId,
                EmployeeName = balance.EmployeeName,

                LeaveTypeId = balance.LeaveTypeId,
                LeaveTypeName = balance.LeaveTypeName,

                Year = balance.Year,

                AllocatedDays = balance.AllocatedDays,
                CarryForwardDays = balance.CarryForwardDays,

                TotalDays = totalDays,
                UsedDays = usedDays,
                ReservedDays = reservedDays,

                RemainingDays =
                    totalDays - usedDays,

                AvailableDays =
                    totalDays - usedDays - reservedDays
            };
        }).ToList();
    }

    private static bool IsValidHalfDayValue(decimal days)
    {
        return days >= 0 &&
               days % 0.5m == 0;
    }
}