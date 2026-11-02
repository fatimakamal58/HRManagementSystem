using HRManagementSystem.Domain.Enums;

namespace HRManagementSystem.Application.DTOs.LeaveTypes;

public class LeaveApprovalStepDto
{
    public int Id { get; set; }

    public int StepOrder { get; set; }

    public ApproverType ApproverType { get; set; }
}