using System.ComponentModel.DataAnnotations;
using HRManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRManagementSystem.Web.ViewModels.EmployeeSalaries;

public class ChangeEmployeeSalaryViewModel
{
    public int EmployeeId { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;

    public decimal CurrentSalary { get; set; }
    
    public List<SelectListItem> ChangeReasons { get; set; } = new();

    [Range(typeof(decimal), "0.01", "9999999999999999",
        ErrorMessage = "Basic salary must be greater than zero.")]
    public decimal BasicSalary { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Please select a change reason.")]
    public SalaryChangeReason? ChangeReason { get; set; }

    [StringLength(250)]
    public string? Notes { get; set; }
}