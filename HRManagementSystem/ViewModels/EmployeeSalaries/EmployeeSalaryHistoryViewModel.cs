using HRManagementSystem.Application.DTOs.EmployeeSalaries;

namespace HRManagementSystem.Web.ViewModels.EmployeeSalaries;

public class EmployeeSalaryHistoryViewModel
{
    public int EmployeeId { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;

    public List<EmployeeSalaryDto> Salaries { get; set; } = new();
    
    public bool IsEmployeeActive { get; set; }
}