using HRManagementSystem.Domain.Common;

namespace HRManagementSystem.Domain.Entities.HR;

public class WorkWeekSetting: BaseEntity
{
    public DayOfWeek DayOfWeek { get; set; }

    public bool IsWorkingDay { get; set; }
}