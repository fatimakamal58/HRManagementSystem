using Microsoft.AspNetCore.Mvc;
using HRManagementSystem.Data;
using HRManagementSystem.Models.HR;
using HRManagementSystem.ViewModels.Employees;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new EmployeeFormViewModel();

            await PopulateListsAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeFormViewModel model)
        {
            if (await _context.Employees
                .AnyAsync(e => e.EmployeeNumber == model.EmployeeNumber))
            {
                ModelState.AddModelError(
                    nameof(model.EmployeeNumber),
                    "رقم الموظف مستخدم مسبقًا.");
            }

            if (await _context.Employees
                .AnyAsync(e => e.NationalId == model.NationalId))
            {
                ModelState.AddModelError(
                    nameof(model.NationalId),
                    "رقم الهوية أو الإقامة مستخدم مسبقًا.");
            }

            if (await _context.Employees
                .AnyAsync(e => e.Email == model.Email))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "البريد الإلكتروني مستخدم مسبقًا.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateListsAsync(model);
                return View(model);
            }

            var employee = new Employee
            {
                CreatedAt = DateTime.UtcNow
            };

            MapEmployee(employee, model);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .OrderBy(e => e.EmployeeNumber)
                .ToListAsync();

            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee is null)
            {
                return NotFound();
            }

            var model = new EmployeeFormViewModel
            {
                Id = employee.Id,
                EmployeeNumber = employee.EmployeeNumber,

                FirstNameAr = employee.FirstNameAr,
                SecondNameAr = employee.SecondNameAr,
                ThirdNameAr = employee.ThirdNameAr,
                LastNameAr = employee.LastNameAr,

                FirstNameEn = employee.FirstNameEn,
                SecondNameEn = employee.SecondNameEn,
                ThirdNameEn = employee.ThirdNameEn,
                LastNameEn = employee.LastNameEn,

                NationalId = employee.NationalId,
                Gender = employee.Gender,
                BirthDate = employee.BirthDate,

                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,

                DepartmentId = employee.DepartmentId,
                JobTitleId = employee.JobTitleId,

                HireDate = employee.HireDate,
                EmploymentStatus = employee.EmploymentStatus,
                IsActive = employee.IsActive
            };

            await PopulateListsAsync(model);

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (await _context.Employees.AnyAsync(
                e => e.EmployeeNumber == model.EmployeeNumber &&
                     e.Id != id))
            {
                ModelState.AddModelError(
                    nameof(model.EmployeeNumber),
                    "رقم الموظف مستخدم مسبقًا.");
            }

            if (await _context.Employees.AnyAsync(
                e => e.NationalId == model.NationalId &&
                     e.Id != id))
            {
                ModelState.AddModelError(
                    nameof(model.NationalId),
                    "رقم الهوية أو الإقامة مستخدم مسبقًا.");
            }

            if (await _context.Employees.AnyAsync(
                e => e.Email == model.Email &&
                     e.Id != id))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "البريد الإلكتروني مستخدم مسبقًا.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateListsAsync(model);
                return View(model);
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee is null)
            {
                return NotFound();
            }

            MapEmployee(employee, model);
            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private static void MapEmployee(Employee employee,EmployeeFormViewModel model)
        {
            employee.EmployeeNumber = model.EmployeeNumber.Trim();

            employee.FirstNameAr = model.FirstNameAr.Trim();
            employee.SecondNameAr = model.SecondNameAr?.Trim();
            employee.ThirdNameAr = model.ThirdNameAr?.Trim();
            employee.LastNameAr = model.LastNameAr.Trim();

            employee.FirstNameEn = model.FirstNameEn.Trim();
            employee.SecondNameEn = model.SecondNameEn?.Trim();
            employee.ThirdNameEn = model.ThirdNameEn?.Trim();
            employee.LastNameEn = model.LastNameEn.Trim();

            employee.NationalId = model.NationalId.Trim();
            employee.Gender = model.Gender!.Value;
            employee.BirthDate = model.BirthDate!.Value;

            employee.PhoneNumber = model.PhoneNumber.Trim();
            employee.Email = model.Email.Trim();

            employee.DepartmentId = model.DepartmentId!.Value;
            employee.JobTitleId = model.JobTitleId!.Value;

            employee.HireDate = model.HireDate!.Value;
            employee.EmploymentStatus = model.EmploymentStatus!.Value;
            employee.IsActive = model.IsActive;
        }

        private async Task PopulateListsAsync(EmployeeFormViewModel model)
        {
            model.Departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.NameAr)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.NameAr
                })
                .ToListAsync();

            model.JobTitles = await _context.JobTitles
                .Where(j => j.IsActive)
                .OrderBy(j => j.NameAr)
                .Select(j => new SelectListItem
                {
                    Value = j.Id.ToString(),
                    Text = j.NameAr
                })
                .ToListAsync();
        }
    }
}
