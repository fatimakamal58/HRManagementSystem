using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.DTOs.Departments
{
    public class DepartmentDto
    {
        public int Id { get; set; }

        public string NameAr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
