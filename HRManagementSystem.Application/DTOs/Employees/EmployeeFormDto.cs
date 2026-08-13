using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.DTOs.Employees
{
    public class EmployeeFormDto
    {
        public CreateEmployeeDto Employee { get; set; } = new();

        public List<LookupDto> Departments { get; set; } = [];

        public List<LookupDto> JobTitles { get; set; } = [];
    }
}
