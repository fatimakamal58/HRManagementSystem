using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Models.Shared
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ScaffoldColumn(false)]
        public DateTime? UpdatedAt { get; set; }
    }
}
