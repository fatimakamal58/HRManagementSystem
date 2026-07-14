using HRManagementSystem.Data;
using HRManagementSystem.Models.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HRManagementSystem.Controllers
{
    // Controller responsible for CRUD operations on Departments.
    // Access is restricted to authenticated users via [Authorize].
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject the database context via DI
        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display the list of departments
        // GET: Departments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Departments.AsNoTracking().ToListAsync());
        }

        // Display details for a specific department
        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id.Value);

            if (department == null) return NotFound();

            return View(department);
        }

        // Show the form to create a new department
        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // Handle form post for creating a new department
        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NameAr,NameEn,DescriptionAr,DescriptionEn,IsActive")] Department department)
        {
            if (ModelState.IsValid)
            {
                department.CreatedAt = DateTime.UtcNow;
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // Show the form to edit an existing department
        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments.FindAsync(id.Value);
            if (department == null) return NotFound();
            return View(department);
        }

        // Handle form post for editing a department
        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    [Bind("Id,NameAr,NameEn,DescriptionAr,DescriptionEn,IsActive")]
    Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            var existingDepartment =
                await _context.Departments.FindAsync(id);

            if (existingDepartment is null)
            {
                return NotFound();
            }

            existingDepartment.NameAr = department.NameAr;
            existingDepartment.NameEn = department.NameEn;
            existingDepartment.DescriptionAr = department.DescriptionAr;
            existingDepartment.DescriptionEn = department.DescriptionEn;
            existingDepartment.IsActive = department.IsActive;
            existingDepartment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Show delete confirmation page
        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id.Value);
            if (department == null) return NotFound();

            return View(department);
        }

        // Handle deletion after confirmation
        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Check if a department exists by id
        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
