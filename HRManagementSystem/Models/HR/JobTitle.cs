using HRManagementSystem.Models.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Models.HR
{
    public class JobTitle : BaseEntity
    {
        [Required(ErrorMessage = "اسم الوظيفة بالعربية مطلوب.")]
        [StringLength(100)]
        public string NameAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job Title name in English is required.")]

        [StringLength(100)]
        public string NameEn { get; set; } = string.Empty;

        [StringLength(500)]
        public string? DescriptionAr { get; set; }

        [StringLength(500)]
        public string? DescriptionEn { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
