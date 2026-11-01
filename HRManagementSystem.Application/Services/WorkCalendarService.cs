using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Infrastructure.Data;

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

        // الحساب حنكتبه هنا

        return 0;
    }
}