namespace HRManagementSystem.Application.Interfaces;

public interface IWorkCalendarService
{
    Task<decimal> CalculateLeaveDaysAsync(
        DateTime startDate,
        DateTime endDate,
        bool includeNonWorkingDays,
        bool includePublicHolidays);
}