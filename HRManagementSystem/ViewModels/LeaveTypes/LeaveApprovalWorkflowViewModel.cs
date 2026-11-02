using HRManagementSystem.Application.DTOs.LeaveTypes;

namespace HRManagementSystem.Web.ViewModels.LeaveTypes;

public class LeaveApprovalWorkflowViewModel
{
    public int LeaveTypeId { get; set; }

    public string LeaveTypeName { get; set; } = string.Empty;

    public List<LeaveApprovalStepDto> Steps { get; set; } = [];

    public CreateLeaveApprovalStepDto NewStep { get; set; } = new();
}