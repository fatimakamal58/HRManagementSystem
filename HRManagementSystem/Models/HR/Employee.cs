using HRManagementSystem.Models.Enums;
using HRManagementSystem.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagementSystem.Models.HR
{
    public class Employee : BaseEntity
    {
        [Required]
        [StringLength(30)]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstNameAr { get; set; } = string.Empty;

        [StringLength(50)]
        public string? SecondNameAr { get; set; }

        [StringLength(50)]
        public string? ThirdNameAr { get; set; }

        [Required]
        [StringLength(50)]
        public string LastNameAr { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstNameEn { get; set; } = string.Empty;

        [StringLength(50)]
        public string? SecondNameEn { get; set; }

        [StringLength(50)]
        public string? ThirdNameEn { get; set; }

        [Required]
        [StringLength(50)]
        public string LastNameEn { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string NationalId { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public Department Department { get; set; } = null!;

        public int JobTitleId { get; set; }

        public JobTitle JobTitle { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

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
