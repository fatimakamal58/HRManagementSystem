using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Application.DTOs.WorkCalendar;

public class WorkWeekSettingDto
{
    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    public bool IsWorkingDay { get; set; }
}