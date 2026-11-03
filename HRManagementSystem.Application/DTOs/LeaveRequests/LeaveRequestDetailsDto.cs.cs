using HRManagementSystem.Domain.Enums;

namespace HRManagementSystem.Application.DTOs.LeaveRequests;

public class LeaveRequestDetailsDto
{
    public int Id { get; set; }

    public string LeaveTypeName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal ChargedDays { get; set; }

    public LeaveRequestStatus Status { get; set; }

    public string? Reason { get; set; }

    public ApproverType? CurrentApproverType { get; set; }

    public List<LeaveApprovalDetailsDto> Approvals { get; set; } = [];
}