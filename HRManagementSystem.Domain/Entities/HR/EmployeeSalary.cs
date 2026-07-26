using HRManagementSystem.Domain.Common;
using HRManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HRManagementSystem.Domain.Entities.HR
{
    public class EmployeeSalary : BaseEntity
    {
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasicSalary { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
        public bool IsCurrent { get; set; }

        public SalaryChangeReason ChangeReason { get; set; }

        [StringLength(250)]
        public string? Notes { get; set; }
    }
}
