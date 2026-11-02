namespace HRManagementSystem.Application.DTOs.LeaveTypes;

public class LeaveTypeDto
{
    public int Id { get; set; }

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public bool RequiresBalance { get; set; }

    public decimal? DefaultDaysPerYear { get; set; }

    public bool AllowCarryForward { get; set; }

    public bool IsActive { get; set; }
}