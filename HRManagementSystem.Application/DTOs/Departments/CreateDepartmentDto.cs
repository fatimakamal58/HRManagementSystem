using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Application.DTOs.Departments
{
    public class CreateDepartmentDto
    {
        [Required(ErrorMessage = "اسم القسم باللغة العربية مطلوب")]
        [StringLength(
            100,
            ErrorMessage = "اسم القسم باللغة العربية يجب ألا يتجاوز 100 حرف")]
        public string NameAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم القسم باللغة الإنجليزية مطلوب")]
        [StringLength(
            100,
            ErrorMessage = "اسم القسم باللغة الإنجليزية يجب ألا يتجاوز 100 حرف")]
        public string NameEn { get; set; } = string.Empty;

        [StringLength(
            500,
            ErrorMessage = "الوصف باللغة العربية يجب ألا يتجاوز 500 حرف")]
        public string? DescriptionAr { get; set; }

        [StringLength(
            500,
            ErrorMessage = "الوصف باللغة الإنجليزية يجب ألا يتجاوز 500 حرف")]
        public string? DescriptionEn { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
