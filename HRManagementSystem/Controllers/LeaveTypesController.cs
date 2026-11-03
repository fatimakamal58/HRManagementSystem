using HRManagementSystem.Application.DTOs.LeaveTypes;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Web.ViewModels.LeaveTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.Controllers;

[Authorize(Roles = "Admin,HR,HRManager")]
public class LeaveTypesController : Controller
{
    private readonly ILeaveTypeService _leaveTypeService;

    public LeaveTypesController(ILeaveTypeService leaveTypeService)
    {
        _leaveTypeService = leaveTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var leaveTypes = await _leaveTypeService.GetAllAsync();

        return View(leaveTypes);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateLeaveTypeDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLeaveTypeDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _leaveTypeService.CreateAsync(model);

            TempData["SuccessMessage"] =
                "تمت إضافة نوع الإجازة بنجاح.";

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Workflow(int id)
    {
        var leaveType = await _leaveTypeService.GetByIdAsync(id);

        if (leaveType is null)
            return NotFound();

        var steps = await _leaveTypeService
            .GetApprovalStepsAsync(id);

        var model = new LeaveApprovalWorkflowViewModel
        {
            LeaveTypeId = leaveType.Id,
            LeaveTypeName = leaveType.NameAr,
            Steps = steps,
            NewStep = new CreateLeaveApprovalStepDto
            {
                LeaveTypeId = leaveType.Id
            }
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddApprovalStep(
    CreateLeaveApprovalStepDto model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] =
                "بيانات خطوة الموافقة غير صحيحة.";

            return RedirectToAction(
                nameof(Workflow),
                new { id = model.LeaveTypeId });
        }

        try
        {
            await _leaveTypeService.AddApprovalStepAsync(model);

            TempData["SuccessMessage"] =
                "تمت إضافة خطوة الموافقة بنجاح.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(
            nameof(Workflow),
            new { id = model.LeaveTypeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateApprovalStep(
    int id,
    int leaveTypeId)
    {
        try
        {
            await _leaveTypeService
                .DeactivateApprovalStepAsync(id);

            TempData["SuccessMessage"] =
                "تم تعطيل خطوة الموافقة بنجاح.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(
            nameof(Workflow),
            new { id = leaveTypeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateApprovalStep(
        int id,
        int leaveTypeId)
    {
        try
        {
            await _leaveTypeService
                .ActivateApprovalStepAsync(id);

            TempData["SuccessMessage"] =
                "تم تفعيل خطوة الموافقة بنجاح.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(
            nameof(Workflow),
            new { id = leaveTypeId });
    }
}