using HRManagementSystem.Application.Interfaces;
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
}