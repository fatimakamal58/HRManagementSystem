using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Application.DTOs.LeaveBalances;

public class SetLeaveBalanceDto
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int LeaveTypeId { get; set; }

    [Required]
    [Range(2000, 2100)]
    public int Year { get; set; }

    [Range(0, 999.99)]
    public decimal AllocatedDays { get; set; }

    [Range(0, 999.99)]
    public decimal CarryForwardDays { get; set; }
}