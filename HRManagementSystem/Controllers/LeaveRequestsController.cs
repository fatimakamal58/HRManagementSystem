using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Web.ViewModels.LeaveRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HRManagementSystem.Controllers;

[Authorize]
public class LeaveRequestsController : Controller
{
    private readonly ILeaveRequestService _leaveRequestService;
    private readonly ILeaveBalanceService _leaveBalanceService;
    private readonly ILeaveTypeService _leaveTypeService;

    public LeaveRequestsController(
        ILeaveRequestService leaveRequestService,
        ILeaveBalanceService leaveBalanceService,
        ILeaveTypeService leaveTypeService)
    {
        _leaveRequestService = leaveRequestService;
        _leaveBalanceService = leaveBalanceService;
        _leaveTypeService = leaveTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
            return Challenge();

        try
        {
            var requests =
                await _leaveRequestService
                    .GetMyRequestsAsync(currentUserId);

            return View(requests);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
            return Challenge();

        var model = new CreateLeaveRequestViewModel();

        try
        {
            await LoadCreateFormAsync(
                model,
                currentUserId);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateLeaveRequestViewModel model)
    {
        var currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
            return Challenge();

        if (!ModelState.IsValid)
        {
            await LoadCreateFormAsync(
                model,
                currentUserId);

            return View(model);
        }

        try
        {
            await _leaveRequestService.CreateAsync(
                model.Request,
                currentUserId);

            TempData["SuccessMessage"] =
                "تم تقديم طلب الإجازة بنجاح.";

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await LoadCreateFormAsync(
                model,
                currentUserId);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
            return Challenge();

        try
        {
            await _leaveRequestService.CancelAsync(
                id,
                currentUserId);

            TempData["SuccessMessage"] =
                "تم إلغاء طلب الإجازة بنجاح.";
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

 

    [HttpGet]
    public async Task<IActionResult> Approvals()
    {
        var currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
            return Challenge();

        var requests =
            await _leaveRequestService
                .GetPendingApprovalsAsync(currentUserId);

        return View(requests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(
    int id,
    string? comment)
    {
        var currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
            return Challenge();

        try
        {
            await _leaveRequestService.ApproveAsync(
                id,
                currentUserId,
                comment);

            TempData["SuccessMessage"] =
                "تم اعتماد الطلب بنجاح.";
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Approvals));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(
    int id,
    string comment)
    {
        var currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
            return Challenge();

        try
        {
            await _leaveRequestService.RejectAsync(
                id,
                currentUserId,
                comment);

            TempData["SuccessMessage"] =
                "تم رفض الطلب.";
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Approvals));
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var currentUserId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
            return Challenge();

        try
        {
            var model = await _leaveRequestService
                .GetDetailsAsync(id, currentUserId);

            if (model is null)
                return NotFound();

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }



    //---------------------------------------------------------

    // Helper method to load leave types for the create form


    private async Task LoadLeaveTypesAsync(
        CreateLeaveRequestViewModel model)
    {
        var leaveTypes =
            await _leaveTypeService.GetAllAsync();

        model.LeaveTypes = leaveTypes
            .Where(x => x.IsActive)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.NameAr
            })
            .ToList();
    }

    private async Task LoadCreateFormAsync(
     CreateLeaveRequestViewModel model,
     string currentUserId)
    {
        await LoadLeaveTypesAsync(model);

        model.Balances =
            await _leaveBalanceService
                .GetMyBalancesAsync(currentUserId);
    }
}