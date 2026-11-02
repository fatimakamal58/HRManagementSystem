namespace HRManagementSystem.Application.DTOs.WorkCalendar;

public class PublicHolidayDto
{
    public int Id { get; set; }

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }
}