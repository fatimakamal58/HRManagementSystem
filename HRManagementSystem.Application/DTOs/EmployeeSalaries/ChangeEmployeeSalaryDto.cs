using HRManagementSystem.Domain.Enums;

namespace HRManagementSystem.Application.DTOs.EmployeeSalaries;

public class ChangeEmployeeSalaryDto
{
    public int EmployeeId { get; set; }

    public decimal BasicSalary { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public SalaryChangeReason ChangeReason { get; set; }

    public string? Notes { get; set; }
}