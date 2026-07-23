using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Application.DTOs.Departments;

public class UpdateDepartmentDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم القسم باللغة العربية مطلوب")]
    [StringLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم القسم باللغة الإنجليزية مطلوب")]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    [StringLength(500)]
    public string? DescriptionAr { get; set; }

    [StringLength(500)]
    public string? DescriptionEn { get; set; }

    public bool IsActive { get; set; }
}