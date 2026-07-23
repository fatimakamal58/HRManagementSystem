using HRManagementSystem.Application.DTOs.Departments;
using HRManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.Controllers
{
    
    // Access is restricted to authenticated users via [Authorize].
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _departmentService.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _departmentService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            var model = new UpdateDepartmentDto
            {
                Id = department.Id,
                NameAr = department.NameAr,
                NameEn = department.NameEn,
                IsActive = department.IsActive
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateDepartmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _departmentService.UpdateAsync(dto);

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }

    }
}
