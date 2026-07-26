using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HRManagementSystem.Application.DTOs.Employees
{
    public class UpdateEmployeeDto
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

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

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly HireDate { get; set; }

        [Required]
        public EmploymentStatus EmploymentStatus { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار القسم.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار المسمى الوظيفي.")]
        public int JobTitleId { get; set; }
    }
}
