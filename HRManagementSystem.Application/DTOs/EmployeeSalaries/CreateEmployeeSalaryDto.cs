using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.DTOs.EmployeeSalaries
{
    public class CreateEmployeeSalaryDto
    {
        public int EmployeeId { get; set; }

        public decimal BasicSalary { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public SalaryChangeReason ChangeReason { get; set; }

        public string? Notes { get; set; }
    }
}
