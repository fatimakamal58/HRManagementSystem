using HRManagementSystem.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.ViewModels.Employees
{
    public class EmployeeFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "رقم الموظف مطلوب.")]
        [StringLength(30, ErrorMessage = "رقم الموظف يجب ألا يتجاوز 30 حرفًا.")]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الأول بالعربية مطلوب.")]
        [StringLength(50)]
        public string FirstNameAr { get; set; } = string.Empty;

        [StringLength(50)]
        public string? SecondNameAr { get; set; }

        [StringLength(50)]
        public string? ThirdNameAr { get; set; }

        [Required(ErrorMessage = "اسم العائلة بالعربية مطلوب.")]
        [StringLength(50)]
        public string LastNameAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الأول بالإنجليزية مطلوب.")]
        [StringLength(50)]
        public string FirstNameEn { get; set; } = string.Empty;

        [StringLength(50)]
        public string? SecondNameEn { get; set; }

        [StringLength(50)]
        public string? ThirdNameEn { get; set; }

        [Required(ErrorMessage = "اسم العائلة بالإنجليزية مطلوب.")]
        [StringLength(50)]
        public string LastNameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهوية أو الإقامة مطلوب.")]
        [StringLength(20, MinimumLength = 5)]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "الجنس مطلوب.")]
        public Gender? Gender { get; set; }

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب.")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب.")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "القسم مطلوب.")]
        public int? DepartmentId { get; set; }

        [Required(ErrorMessage = "المسمى الوظيفي مطلوب.")]
        public int? JobTitleId { get; set; }

        [Required(ErrorMessage = "تاريخ التوظيف مطلوب.")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        [Required(ErrorMessage = "حالة الموظف مطلوبة.")]
        public EmploymentStatus? EmploymentStatus { get; set; }

        public bool IsActive { get; set; } = true;

        public IEnumerable<SelectListItem> Departments { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> JobTitles { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}
