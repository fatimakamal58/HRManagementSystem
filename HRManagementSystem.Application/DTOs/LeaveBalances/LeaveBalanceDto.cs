namespace HRManagementSystem.Application.DTOs.LeaveBalances;

public class LeaveBalanceDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;

    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;

    public int Year { get; set; }

    public decimal AllocatedDays { get; set; }
    public decimal CarryForwardDays { get; set; }

    public decimal TotalDays { get; set; }

    public decimal UsedDays { get; set; }

    public decimal ReservedDays { get; set; }

    public decimal RemainingDays { get; set; }

    public decimal AvailableDays { get; set; }
}