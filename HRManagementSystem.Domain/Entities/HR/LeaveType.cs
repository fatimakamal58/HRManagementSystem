using System.ComponentModel.DataAnnotations;
using HRManagementSystem.Domain.Common;

namespace HRManagementSystem.Domain.Entities.HR;

public class LeaveType : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    public bool RequiresBalance { get; set; } = true;
    
    public bool IncludeNonWorkingDays { get; set; } = true;

    public bool IncludePublicHolidays { get; set; } = true;

    public decimal? DefaultDaysPerYear { get; set; }

    public bool AllowCarryForward { get; set; }

    public decimal? MaxCarryForwardDays { get; set; }

    public ICollection<LeaveBalance> LeaveBalances { get; set; }
        = new List<LeaveBalance>();

    public ICollection<LeaveRequest> LeaveRequests { get; set; }
        = new List<LeaveRequest>();

    public ICollection<LeaveApprovalStep> ApprovalSteps { get; set; }
        = new List<LeaveApprovalStep>();
}