using System.ComponentModel.DataAnnotations;
using HRManagementSystem.Domain.Common;

namespace HRManagementSystem.Domain.Entities.HR;

public class PublicHoliday : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
