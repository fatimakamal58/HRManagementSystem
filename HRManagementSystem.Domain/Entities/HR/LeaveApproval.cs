using System.ComponentModel.DataAnnotations;
using HRManagementSystem.Domain.Common;
using HRManagementSystem.Domain.Enums;

namespace HRManagementSystem.Domain.Entities.HR;

public class LeaveApproval : BaseEntity
{
    public int LeaveRequestId { get; set; }
    public LeaveRequest LeaveRequest { get; set; } = null!;

    public int StepOrder { get; set; }

    public ApproverType ApproverType { get; set; }

    public string? ApproverUserId { get; set; }

    public ApprovalStatus Status { get; set; }
        = ApprovalStatus.Pending;

    public DateTime? ActionAt { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }
}