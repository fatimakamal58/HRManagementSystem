using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Application.DTOs.WorkCalendar;

public class CreatePublicHolidayDto
{
    [Required]
    [StringLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }
}