using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Application.DTOs.LeaveBalances;

public class UpdateLeaveBalanceDto
{
    [Required]
    public int Id { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string LeaveTypeName { get; set; } = string.Empty;

    public int Year { get; set; }

    [Range(0, 999.99)]
    public decimal AllocatedDays { get; set; }

    [Range(0, 999.99)]
    public decimal CarryForwardDays { get; set; }
}