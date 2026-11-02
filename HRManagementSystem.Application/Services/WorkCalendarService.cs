using HRManagementSystem.Application.DTOs.WorkCalendar;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities.HR;
using HRManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Application.Services;

public class WorkCalendarService : IWorkCalendarService
{
    private readonly ApplicationDbContext _context;

    public WorkCalendarService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> CalculateLeaveDaysAsync(
     DateTime startDate,
     DateTime endDate,
     bool includeNonWorkingDays,
     bool includePublicHolidays)
    {
        startDate = startDate.Date;
        endDate = endDate.Date;

        if (endDate < startDate)
        {
            throw new ArgumentException(
                "End date cannot be before start date.");
        }

        var workWeekSettings = await _context.WorkWeekSettings
            .AsNoTracking()
            .ToDictionaryAsync(
                x => x.DayOfWeek,
                x => x.IsWorkingDay);

        var publicHolidays = await _context.PublicHolidays
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                x.StartDate <= endDate &&
                x.EndDate >= startDate)
            .ToListAsync();

        decimal leaveDays = 0;

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (!workWeekSettings.TryGetValue(date.DayOfWeek, out var isWorkingDay))
            {
                throw new InvalidOperationException(
                    $"Work week setting for {date.DayOfWeek} is not configured.");
            }

            if (!includeNonWorkingDays && !isWorkingDay)
            {
                continue;
            }

            var isPublicHoliday = publicHolidays.Any(x =>
                x.StartDate.Date <= date &&
                x.EndDate.Date >= date);

            if (!includePublicHolidays && isPublicHoliday)
            {
                continue;
            }

            leaveDays++;
        }

        return leaveDays;
    }

    public async Task<List<WorkWeekSettingDto>> GetWorkWeekAsync()
    {
        return await _context.WorkWeekSettings
            .AsNoTracking()
            .OrderBy(x => x.DayOfWeek)
            .Select(x => new WorkWeekSettingDto
            {
                DayOfWeek = x.DayOfWeek,
                IsWorkingDay = x.IsWorkingDay
            })
            .ToListAsync();
    }

    public async Task UpdateWorkWeekAsync(
    List<WorkWeekSettingDto> settings)
    {
        if (settings.Count != 7)
            throw new InvalidOperationException(
                "Work week must contain all seven days.");

        var distinctDays = settings
            .Select(x => x.DayOfWeek)
            .Distinct()
            .Count();

        if (distinctDays != 7)
            throw new InvalidOperationException(
                "Work week contains duplicate or missing days.");

        if (!settings.Any(x => x.IsWorkingDay))
            throw new InvalidOperationException(
                "At least one working day must be configured.");

        var existingSettings =
            await _context.WorkWeekSettings.ToListAsync();

        foreach (var setting in settings)
        {
            var existing = existingSettings
                .FirstOrDefault(x =>
                    x.DayOfWeek == setting.DayOfWeek);

            if (existing is null)
            {
                _context.WorkWeekSettings.Add(
                    new WorkWeekSetting
                    {
                        DayOfWeek = setting.DayOfWeek,
                        IsWorkingDay = setting.IsWorkingDay,
                        IsActive = true
                    });
            }
            else
            {
                existing.IsWorkingDay = setting.IsWorkingDay;
                existing.IsActive = true;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task CreatePublicHolidayAsync(
    CreatePublicHolidayDto dto)
    {
        var startDate = dto.StartDate.Date;
        var endDate = dto.EndDate.Date;

        if (endDate < startDate)
            throw new InvalidOperationException(
                "End date cannot be before start date.");

        var hasOverlap = await _context.PublicHolidays
            .AnyAsync(x =>
                x.IsActive &&
                x.StartDate <= endDate &&
                x.EndDate >= startDate);

        if (hasOverlap)
            throw new InvalidOperationException(
                "The holiday overlaps with an existing public holiday.");

        var holiday = new PublicHoliday
        {
            NameAr = dto.NameAr.Trim(),
            NameEn = dto.NameEn.Trim(),
            StartDate = startDate,
            EndDate = endDate,
            IsActive = true
        };

        _context.PublicHolidays.Add(holiday);

        await _context.SaveChangesAsync();
    }
    public async Task<List<PublicHolidayDto>> GetPublicHolidaysAsync()
    {
        return await _context.PublicHolidays
            .AsNoTracking()
            .OrderByDescending(x => x.StartDate)
            .Select(x => new PublicHolidayDto
            {
                Id = x.Id,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<UpdatePublicHolidayDto?> GetPublicHolidayForUpdateAsync(
    int id)
    {
        return await _context.PublicHolidays
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new UpdatePublicHolidayDto
            {
                Id = x.Id,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdatePublicHolidayAsync(
    UpdatePublicHolidayDto dto)
    {
        var holiday = await _context.PublicHolidays
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (holiday is null)
            throw new InvalidOperationException(
                "Public holiday was not found.");

        var nameAr = dto.NameAr.Trim();
        var nameEn = dto.NameEn.Trim();

        if (string.IsNullOrWhiteSpace(nameAr) ||
            string.IsNullOrWhiteSpace(nameEn))
            throw new InvalidOperationException(
                "Holiday names are required.");

        var startDate = dto.StartDate.Date;
        var endDate = dto.EndDate.Date;

        if (endDate < startDate)
            throw new InvalidOperationException(
                "End date cannot be before start date.");

        var hasOverlap = await _context.PublicHolidays
            .AnyAsync(x =>
                x.Id != dto.Id &&
                x.IsActive &&
                x.StartDate <= endDate &&
                x.EndDate >= startDate);

        if (hasOverlap)
            throw new InvalidOperationException(
                "The holiday overlaps with an existing public holiday.");

        holiday.NameAr = nameAr;
        holiday.NameEn = nameEn;
        holiday.StartDate = startDate;
        holiday.EndDate = endDate;

        await _context.SaveChangesAsync();
    }

    public async Task DeactivatePublicHolidayAsync(int id)
    {
        var holiday = await _context.PublicHolidays
            .FirstOrDefaultAsync(x => x.Id == id);

        if (holiday is null)
            throw new InvalidOperationException(
                "Public holiday was not found.");

        if (!holiday.IsActive)
            throw new InvalidOperationException(
                "Public holiday is already inactive.");

        holiday.IsActive = false;

        await _context.SaveChangesAsync();
    }

    public async Task ActivatePublicHolidayAsync(int id)
    {
        var holiday = await _context.PublicHolidays
            .FirstOrDefaultAsync(x => x.Id == id);

        if (holiday is null)
            throw new InvalidOperationException(
                "Public holiday was not found.");

        if (holiday.IsActive)
            throw new InvalidOperationException(
                "Public holiday is already active.");

        var hasOverlap = await _context.PublicHolidays
            .AnyAsync(x =>
                x.Id != holiday.Id &&
                x.IsActive &&
                x.StartDate <= holiday.EndDate &&
                x.EndDate >= holiday.StartDate);

        if (hasOverlap)
            throw new InvalidOperationException(
                "Public holiday cannot be activated because its dates overlap with another active holiday.");

        holiday.IsActive = true;

        await _context.SaveChangesAsync();
    }
}