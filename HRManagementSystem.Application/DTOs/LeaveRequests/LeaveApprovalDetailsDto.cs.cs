using HRManagementSystem.Domain.Enums;

namespace HRManagementSystem.Application.DTOs.LeaveRequests;

public class LeaveApprovalDetailsDto
{
    public int StepOrder { get; set; }

    public ApproverType ApproverType { get; set; }

    public ApprovalStatus Status { get; set; }

    public DateTime? ActionAt { get; set; }

    public string? Comment { get; set; }
}