using HRManagementSystem.Domain.Common;
using HRManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Domain.Entities.HR;

public class LeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public int LeaveTypeId { get; set; }
    public LeaveType LeaveType { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal ChargedDays { get; set; }

    public LeaveRequestStatus Status { get; set; }
        = LeaveRequestStatus.Pending;

    [StringLength(500)]
    public string? Reason { get; set; }

    public ICollection<LeaveApproval> Approvals { get; set; }
        = new List<LeaveApproval>();
}