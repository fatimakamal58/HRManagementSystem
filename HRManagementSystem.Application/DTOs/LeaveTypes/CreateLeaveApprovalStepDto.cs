using HRManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Application.DTOs.LeaveTypes;

public class CreateLeaveApprovalStepDto
{
    [Required]
    public int LeaveTypeId { get; set; }

    [Required]
    public ApproverType ApproverType { get; set; }
}