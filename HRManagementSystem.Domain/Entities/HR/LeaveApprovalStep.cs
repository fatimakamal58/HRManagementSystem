using HRManagementSystem.Domain.Common;
using HRManagementSystem.Domain.Enums;

namespace HRManagementSystem.Domain.Entities.HR;

public class LeaveApprovalStep : BaseEntity
{
    public int LeaveTypeId { get; set; }
    public LeaveType LeaveType { get; set; } = null!;

    public int StepOrder { get; set; }

    public ApproverType ApproverType { get; set; }
}