using HRManagementSystem.Models.Enums;
using HRManagementSystem.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagementSystem.Models.HR
{
    public class Employee : BaseEntity
    {
        [Required(ErrorMessage = "رقم الموظف مطلوب.")]
        [StringLength(
            30,
            ErrorMessage = "رقم الموظف يجب ألا يتجاوز 30 حرفًا."
        )]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الأول بالعربية مطلوب.")]
        [StringLength(
            50,
            ErrorMessage = "الاسم الأول بالعربية يجب ألا يتجاوز 50 حرفًا."
        )]
        public string FirstNameAr { get; set; } = string.Empty;

        [StringLength(
            50,
            ErrorMessage = "الاسم الثاني بالعربية يجب ألا يتجاوز 50 حرفًا."
        )]
        public string? SecondNameAr { get; set; }

        [StringLength(
            50,
            ErrorMessage = "الاسم الثالث بالعربية يجب ألا يتجاوز 50 حرفًا."
        )]
        public string? ThirdNameAr { get; set; }

        [Required(ErrorMessage = "اسم العائلة بالعربية مطلوب.")]
        [StringLength(
            50,
            ErrorMessage = "اسم العائلة بالعربية يجب ألا يتجاوز 50 حرفًا."
        )]
        public string LastNameAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الأول بالإنجليزية مطلوب.")]
        [StringLength(
            50,
            ErrorMessage = "الاسم الأول بالإنجليزية يجب ألا يتجاوز 50 حرفًا."
        )]
        public string FirstNameEn { get; set; } = string.Empty;

        [StringLength(
            50,
            ErrorMessage = "الاسم الثاني بالإنجليزية يجب ألا يتجاوز 50 حرفًا."
        )]
        public string? SecondNameEn { get; set; }

        [StringLength(
            50,
            ErrorMessage = "الاسم الثالث بالإنجليزية يجب ألا يتجاوز 50 حرفًا."
        )]
        public string? ThirdNameEn { get; set; }

        [Required(ErrorMessage = "اسم العائلة بالإنجليزية مطلوب.")]
        [StringLength(
            50,
            ErrorMessage = "اسم العائلة بالإنجليزية يجب ألا يتجاوز 50 حرفًا."
        )]
        public string LastNameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهوية أو الإقامة مطلوب.")]
        [StringLength(
            20,
            MinimumLength = 5,
            ErrorMessage = "رقم الهوية أو الإقامة يجب أن يكون بين 5 و20 حرفًا."
        )]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "الجنس مطلوب.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب.")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب.")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح.")]
        [StringLength(
            20,
            ErrorMessage = "رقم الهاتف يجب ألا يتجاوز 20 حرفًا."
        )]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        [StringLength(
            150,
            ErrorMessage = "البريد الإلكتروني يجب ألا يتجاوز 150 حرفًا."
        )]
        public string Email { get; set; } = string.Empty;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "يرجى اختيار القسم."
        )]
        public int DepartmentId { get; set; }

        public Department Department { get; set; } = null!;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "يرجى اختيار المسمى الوظيفي."
        )]
        public int JobTitleId { get; set; }

        public JobTitle JobTitle { get; set; } = null!;

        [Required(ErrorMessage = "تاريخ التوظيف مطلوب.")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "حالة الموظف مطلوبة.")]
        public EmploymentStatus EmploymentStatus { get; set; }
            = EmploymentStatus.Active;

        public bool IsActive { get; set; } = true;

        [NotMapped]
        public string FullNameAr =>
            string.Join(
                " ",
                new[]
                {
                FirstNameAr,
                SecondNameAr,
                ThirdNameAr,
                LastNameAr
                }.Where(name => !string.IsNullOrWhiteSpace(name)));

        [NotMapped]
        public string FullNameEn =>
            string.Join(
                " ",
                new[]
                {
                FirstNameEn,
                SecondNameEn,
                ThirdNameEn,
                LastNameEn
                }.Where(name => !string.IsNullOrWhiteSpace(name)));
    }
}
