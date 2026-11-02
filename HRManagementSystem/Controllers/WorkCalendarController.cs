using HRManagementSystem.Application.DTOs.WorkCalendar;
using HRManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.Controllers;

[Authorize(Roles = "Admin,HR,HRManager")]
public class WorkCalendarController : Controller
{
    private readonly IWorkCalendarService _workCalendarService;

    public WorkCalendarController(
        IWorkCalendarService workCalendarService)
    {
        _workCalendarService = workCalendarService;
    }

    [HttpGet]
    public async Task<IActionResult> WorkWeek()
    {
        var settings =
            await _workCalendarService.GetWorkWeekAsync();

        // أول مرة فقط: الجدول سيكون فارغًا
        if (settings.Count == 0)
        {
            settings = Enum.GetValues<DayOfWeek>()
                .Select(day => new WorkWeekSettingDto
                {
                    DayOfWeek = day,
                    IsWorkingDay = false
                })
                .ToList();
        }

        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WorkWeek(
        List<WorkWeekSettingDto> model)
    {
        try
        {
            await _workCalendarService.UpdateWorkWeekAsync(model);

            TempData["SuccessMessage"] =
                "تم حفظ إعدادات أسبوع العمل بنجاح.";

            return RedirectToAction(nameof(WorkWeek));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> PublicHolidays()
    {
        var holidays =
            await _workCalendarService.GetPublicHolidaysAsync();

        return View(holidays);
    }

    [HttpGet]
    public IActionResult CreatePublicHoliday()
    {
        return View(new CreatePublicHolidayDto());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePublicHoliday(
    CreatePublicHolidayDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _workCalendarService
                .CreatePublicHolidayAsync(model);

            TempData["SuccessMessage"] =
                "تمت إضافة العطلة الرسمية بنجاح.";

            return RedirectToAction(nameof(PublicHolidays));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditPublicHoliday(int id)
    {
        var holiday =
            await _workCalendarService.GetPublicHolidayForUpdateAsync(id);

        if (holiday is null)
            return NotFound();

        return View(holiday);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPublicHoliday(
    UpdatePublicHolidayDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _workCalendarService.UpdatePublicHolidayAsync(model);

            TempData["SuccessMessage"] =
                "تم تعديل العطلة الرسمية بنجاح.";

            return RedirectToAction(nameof(PublicHolidays));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivatePublicHoliday(int id)
    {
        try
        {
            await _workCalendarService.DeactivatePublicHolidayAsync(id);

            TempData["SuccessMessage"] =
                "تم تعطيل العطلة الرسمية بنجاح.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(PublicHolidays));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivatePublicHoliday(int id)
    {
        try
        {
            await _workCalendarService.ActivatePublicHolidayAsync(id);

            TempData["SuccessMessage"] =
                "تم تفعيل العطلة الرسمية بنجاح.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(PublicHolidays));
    }
}