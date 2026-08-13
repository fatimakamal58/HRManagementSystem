using HRManagementSystem.Application.DTOs.Employees;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Web.ViewModels.Employees;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRManagementSystem.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(
        IEmployeeService employeeService
        )
    {
        _employeeService = employeeService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var employees = await _employeeService.GetAllAsync();

        return View(employees);
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);

        return View(employee);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = await _employeeService.GetCreateFormAsync();

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormDto model)
    {
        if (!ModelState.IsValid)
        {
            var formData = await _employeeService.GetCreateFormAsync();

            model.Departments = formData.Departments;
            model.JobTitles = formData.JobTitles;

            return View(model);
        }

        await _employeeService.CreateAsync(model.Employee);

        TempData["SuccessMessage"] = "تمت إضافة الموظف بنجاح.";

        return RedirectToAction(nameof(Index));
    }


}