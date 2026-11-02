namespace HRManagementSystem.Application.DTOs.LeaveBalances;

public class InitializeLeaveBalanceDto
{
    public int Year { get; set; }

    public List<LeaveTypeInitializationDto> LeaveTypes { get; set; } = [];
}

public class LeaveTypeInitializationDto
{
    public int LeaveTypeId { get; set; }

    public string LeaveTypeName { get; set; } = string.Empty;

    public decimal Days { get; set; }
}