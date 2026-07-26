using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.DTOs.Employees
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        public string EmployeeNumber { get; set; } = string.Empty;

        public string FullNameAr { get; set; } = string.Empty;

        public string FullNameEn { get; set; } = string.Empty;

        public string NationalId { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        public DateOnly BirthDate { get; set; }

        public DateOnly HireDate { get; set; }

        public EmploymentStatus EmploymentStatus { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int JobTitleId { get; set; }

        public string JobTitleName { get; set; } = string.Empty;
    }
}
