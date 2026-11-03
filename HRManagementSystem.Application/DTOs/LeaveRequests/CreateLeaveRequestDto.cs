using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Application.DTOs.LeaveRequests;

public class CreateLeaveRequestDto
{

    [Required]
    public int LeaveTypeId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Reason { get; set; }
}