using HRManagementSystem.Application.DTOs.EmployeeSalaries;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Web.ViewModels.EmployeeSalaries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRManagementSystem.Web.Controllers;

public class EmployeeSalaryController : Controller
{
    private readonly IEmployeeSalaryService _salaryService;
    private readonly IEmployeeService _employeeService;

    public EmployeeSalaryController(
        IEmployeeSalaryService salaryService, 
        IEmployeeService  employeeService)
    {
        _salaryService = salaryService;
        _employeeService = employeeService;
    }
    [HttpGet]
    public async Task<IActionResult> History(int employeeId)
    {
        var employee =
            await _employeeService.GetByIdIncludingInactiveAsync(employeeId);

        var salaries = await _salaryService
            .GetSalaryHistoryAsync(employeeId);

        var viewModel = new EmployeeSalaryHistoryViewModel
        {
            EmployeeId = employee.Id,
            EmployeeNumber = employee.EmployeeNumber,
            EmployeeName = employee.FullNameAr,
            IsEmployeeActive = employee.IsActive,
            Salaries = salaries
        };

        return View(viewModel);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create(int employeeId)
    {
        var employee = await _employeeService.GetByIdAsync(employeeId);

        var model = new CreateEmployeeSalaryViewModel
        {
            EmployeeId = employee.Id,
            EmployeeNumber = employee.EmployeeNumber,
            EmployeeName = employee.FullNameAr,
            EffectiveFrom = DateTime.Today
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateEmployeeSalaryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateEmployeeInfoAsync(model);
            return View(model);
        }

        var dto = new CreateInitialSalaryDto
        {
            EmployeeId = model.EmployeeId,
            BasicSalary = model.BasicSalary,
            EffectiveFrom = model.EffectiveFrom,
            Notes = model.Notes
        };

        await _salaryService.CreateInitialSalaryAsync(dto);

        TempData["SuccessMessage"] =
            "Initial salary added successfully.";

        return RedirectToAction(
            nameof(History),
            new { employeeId = model.EmployeeId });
    }
    
    [HttpGet]
    public async Task<IActionResult> Change(int employeeId)
    {
        var employee = await _employeeService.GetByIdAsync(employeeId);

        var currentSalary =
            await _salaryService.GetCurrentSalaryAsync(employeeId);

        if (currentSalary is null)
        {
            return RedirectToAction(
                nameof(Create),
                new { employeeId });
        }

        var model = new ChangeEmployeeSalaryViewModel
        {
            EmployeeId = employee.Id,
            EmployeeNumber = employee.EmployeeNumber,
            EmployeeName = employee.FullNameAr,
            CurrentSalary = currentSalary.BasicSalary,
            EffectiveFrom = DateTime.Today,
            ChangeReasons = GetSalaryChangeReasons()
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Change(
        ChangeEmployeeSalaryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateChangeEmployeeInfoAsync(model);
            return View(model);
        }

        var dto = new ChangeEmployeeSalaryDto
        {
            EmployeeId = model.EmployeeId,
            BasicSalary = model.BasicSalary,
            EffectiveFrom = model.EffectiveFrom,
            ChangeReason = model.ChangeReason!.Value,
            Notes = model.Notes,
        };

        try
        {
            await _salaryService.ChangeSalaryAsync(dto);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await PopulateChangeEmployeeInfoAsync(model);

            return View(model);
        }

        TempData["SuccessMessage"] =
            "Salary changed successfully.";

        return RedirectToAction(
            nameof(History),
            new { employeeId = model.EmployeeId });
    }
    
    //********* private methods **********
    private async Task PopulateEmployeeInfoAsync(
        CreateEmployeeSalaryViewModel model)
    {
        var employee =
            await _employeeService.GetByIdAsync(model.EmployeeId);

        model.EmployeeNumber = employee.EmployeeNumber;
        model.EmployeeName = employee.FullNameAr;
    }
    private async Task PopulateChangeEmployeeInfoAsync(
        ChangeEmployeeSalaryViewModel model)
    {
        var employee =
            await _employeeService.GetByIdAsync(model.EmployeeId);

        var currentSalary =
            await _salaryService.GetCurrentSalaryAsync(model.EmployeeId);

        model.EmployeeNumber = employee.EmployeeNumber;
        model.EmployeeName = employee.FullNameAr;
        model.CurrentSalary = currentSalary?.BasicSalary ?? 0;
        model.ChangeReasons = GetSalaryChangeReasons();
    }
    private static List<SelectListItem> GetSalaryChangeReasons()
    {
        return Enum.GetValues<SalaryChangeReason>()
            .Where(r => r != SalaryChangeReason.NewHire)
            .Select(r => new SelectListItem
            {
                Value = ((int)r).ToString(),
                Text = r.ToString()
            })
            .ToList();
    }
}