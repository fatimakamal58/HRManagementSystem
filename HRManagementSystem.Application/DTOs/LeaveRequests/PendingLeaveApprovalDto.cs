using HRManagementSystem.Domain.Enums;

namespace HRManagementSystem.Application.DTOs.LeaveRequests;

public class PendingLeaveApprovalDto
{
    public int LeaveRequestId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string LeaveTypeName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal ChargedDays { get; set; }

    public string? Reason { get; set; }

    public int StepOrder { get; set; }

    public ApproverType ApproverType { get; set; }
}