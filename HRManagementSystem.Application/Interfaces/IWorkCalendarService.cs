using HRManagementSystem.Application.DTOs.WorkCalendar;

namespace HRManagementSystem.Application.Interfaces;

public interface IWorkCalendarService
{
    Task<decimal> CalculateLeaveDaysAsync(
        DateTime startDate,
        DateTime endDate,
        bool includeNonWorkingDays,
        bool includePublicHolidays);

    Task<List<WorkWeekSettingDto>> GetWorkWeekAsync();

    Task UpdateWorkWeekAsync(
        List<WorkWeekSettingDto> settings);
    Task CreatePublicHolidayAsync(CreatePublicHolidayDto dto);
    Task<List<PublicHolidayDto>> GetPublicHolidaysAsync();
    Task<UpdatePublicHolidayDto?> GetPublicHolidayForUpdateAsync(int id);

    Task UpdatePublicHolidayAsync(UpdatePublicHolidayDto dto);

    Task DeactivatePublicHolidayAsync(int id);

    Task ActivatePublicHolidayAsync(int id);
}