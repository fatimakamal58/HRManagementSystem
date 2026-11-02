using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Application.DTOs.LeaveTypes;

public class CreateLeaveTypeDto
{
    [Required]
    [StringLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    public bool RequiresBalance { get; set; } = true;

    public bool IncludeNonWorkingDays { get; set; }

    public bool IncludePublicHolidays { get; set; }

    public decimal? DefaultDaysPerYear { get; set; }

    public bool AllowCarryForward { get; set; }

    public decimal? MaxCarryForwardDays { get; set; }
}