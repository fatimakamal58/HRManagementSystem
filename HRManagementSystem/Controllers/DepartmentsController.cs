using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRManagementSystem.Application.Interfaces;

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

        
        
    }
}
