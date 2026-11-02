using HRManagementSystem.Application.DTOs.LeaveBalances;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRManagementSystem.Web.ViewModels.LeaveBalances;

public class LeaveBalanceFormViewModel
{
    public SetLeaveBalanceDto Balance { get; set; } = new();

    public List<SelectListItem> Employees { get; set; } = [];

    public List<SelectListItem> LeaveTypes { get; set; } = [];
}