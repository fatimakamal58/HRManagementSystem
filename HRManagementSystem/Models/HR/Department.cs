using HRManagementSystem.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Models.HR
{
    public class Department : BaseEntity
    {

        [Required(ErrorMessage = "اسم القسم بالعربية مطلوب.")]
        [StringLength(100)]
        public string NameAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department name in English is required.")]
        [StringLength(100)]
        public string NameEn { get; set; } = string.Empty;

        [StringLength(300)]
        public string? DescriptionAr { get; set; }

        [StringLength(300)]
        public string? DescriptionEn { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
