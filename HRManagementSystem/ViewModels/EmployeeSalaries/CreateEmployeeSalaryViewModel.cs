using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.ViewModels.EmployeeSalaries;

public class CreateEmployeeSalaryViewModel
{
    public int EmployeeId { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999999999",
        ErrorMessage = "Basic salary must be greater than zero.")]
    public decimal BasicSalary { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;

    [StringLength(250)]
    public string? Notes { get; set; }
}