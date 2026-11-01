using HRManagementSystem.Domain.Common;

namespace HRManagementSystem.Domain.Entities.HR;


public class LeaveBalance : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public int LeaveTypeId { get; set; }
    public LeaveType LeaveType { get; set; } = null!;

    public int Year { get; set; }

    public decimal AllocatedDays { get; set; }

    public decimal CarryForwardDays { get; set; }
}