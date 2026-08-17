using HRManagementSystem.Application.DTOs.Employees;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Application.Services;
using HRManagementSystem.Web.ViewModels.Employees;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRManagementSystem.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly IJobTitleService _jobTitleService;

    public EmployeesController(
    IEmployeeService employeeService,
    IDepartmentService departmentService,
    IJobTitleService jobTitleService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        _jobTitleService = jobTitleService;
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
        var model = new CreateEmployeeViewModel();

        await FillLookupsAsync(model);

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEmployeeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await FillLookupsAsync(model);

            return View(model);
        }

        await _employeeService.CreateAsync(model.Employee);

        TempData["SuccessMessage"] = "تمت إضافة الموظف بنجاح.";

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetForUpdateAsync(id);

        var model = new UpdateEmployeeViewModel
        {
            Employee = employee
        };

        await FillLookupsAsync(model);

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateEmployeeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await FillLookupsAsync(model);

            return View(model);
        }

        await _employeeService.UpdateAsync(model.Employee);

        TempData["SuccessMessage"] = "تم تعديل بيانات الموظف بنجاح.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteAsync(id);

        TempData["SuccessMessage"] = "تم حذف الموظف بنجاح.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Deleted()
    {
        var employees = await _employeeService.GetDeletedAsync();

        return View(employees);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id)
    {
        await _employeeService.RestoreAsync(id);

        TempData["SuccessMessage"] =
            "تمت استعادة الموظف بنجاح.";

        return RedirectToAction(nameof(Deleted));
    }




    // function to fill the dropdowns for departments and job titles
    private async Task FillLookupsAsync(
    CreateEmployeeViewModel model)
    {
        var departments = await _departmentService.GetLookupAsync();
        var jobTitles = await _jobTitleService.GetLookupAsync();

        model.Departments = departments
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();

        model.JobTitles = jobTitles
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();
    }
    //overloaded function to fill the dropdowns for departments and job titles for update view model
    private async Task FillLookupsAsync(
   UpdateEmployeeViewModel model)
    {
        var departments = await _departmentService.GetLookupAsync();
        var jobTitles = await _jobTitleService.GetLookupAsync();

        model.Departments = departments
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();

        model.JobTitles = jobTitles
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();
    }

}