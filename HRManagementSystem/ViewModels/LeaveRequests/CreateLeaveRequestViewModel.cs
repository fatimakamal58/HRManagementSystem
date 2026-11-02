using HRManagementSystem.Application.DTOs.LeaveBalances;
using HRManagementSystem.Application.DTOs.LeaveRequests;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRManagementSystem.Web.ViewModels.LeaveRequests;

public class CreateLeaveRequestViewModel
{
    public CreateLeaveRequestDto Request { get; set; } = new();

    public List<SelectListItem> LeaveTypes { get; set; } = [];

    public List<LeaveBalanceDto> Balances { get; set; } = [];
}