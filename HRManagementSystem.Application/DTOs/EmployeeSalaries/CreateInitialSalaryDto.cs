namespace HRManagementSystem.Application.DTOs.EmployeeSalaries;

public class CreateInitialSalaryDto
{
    public int EmployeeId { get; set; }

    public decimal BasicSalary { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public string? Notes { get; set; }
}