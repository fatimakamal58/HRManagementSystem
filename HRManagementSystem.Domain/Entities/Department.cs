using HRManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string NameAr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;

        public string? DescriptionAr { get; set; }

        public string? DescriptionEn { get; set; }

        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}
