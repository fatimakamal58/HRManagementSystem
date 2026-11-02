using HRManagementSystem.Application.DTOs.LeaveBalances;
using HRManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.Controllers;

[Authorize(Roles = "Admin,HR,HRManager")]
public class LeaveBalancesController : Controller
{
    private readonly ILeaveBalanceService _leaveBalanceService;

    public LeaveBalancesController(
        ILeaveBalanceService leaveBalanceService)
    {
        _leaveBalanceService = leaveBalanceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var balances =
            await _leaveBalanceService.GetAllAsync();

        return View(balances);
    }

    [HttpGet]
    public async Task<IActionResult> InitializeYear(int year)
    {
        try
        {
            var model = await _leaveBalanceService
                .GetInitializationFormAsync(year);

            return View(model);
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;

            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InitializeYear(
     InitializeLeaveBalanceDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _leaveBalanceService
                .InitializeYearBalancesAsync(model);

            TempData["SuccessMessage"] =
                $"تمت تهيئة أرصدة الإجازات لسنة {model.Year} بنجاح.";

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var balance =
            await _leaveBalanceService.GetForUpdateAsync(id);

        if (balance is null)
            return NotFound();

        return View(balance);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    UpdateLeaveBalanceDto model)
    {
        if (!ModelState.IsValid)
        {
            var exists =
                await LoadBalanceDisplayDataAsync(model);

            if (!exists)
                return NotFound();

            return View(model);
        }

        try
        {
            await _leaveBalanceService.UpdateAsync(model);

            TempData["SuccessMessage"] =
                "تم تعديل رصيد الإجازة بنجاح.";

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            var exists =
                await LoadBalanceDisplayDataAsync(model);

            if (!exists)
                return NotFound();

            return View(model);
        }
    }


    private async Task<bool> LoadBalanceDisplayDataAsync(
    UpdateLeaveBalanceDto model)
    {
        var existingBalance =
            await _leaveBalanceService.GetForUpdateAsync(model.Id);

        if (existingBalance is null)
            return false;

        model.EmployeeName = existingBalance.EmployeeName;
        model.LeaveTypeName = existingBalance.LeaveTypeName;
        model.Year = existingBalance.Year;

        return true;
    }
}