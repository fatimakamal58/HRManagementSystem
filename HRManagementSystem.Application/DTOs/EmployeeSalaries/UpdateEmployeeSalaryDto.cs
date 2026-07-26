using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.DTOs.EmployeeSalaries
{
    public class UpdateEmployeeSalaryDto
    {
        public int Id { get; set; }

        public decimal BasicSalary { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public SalaryChangeReason ChangeReason { get; set; }

        public string? Notes { get; set; }
    }
}
