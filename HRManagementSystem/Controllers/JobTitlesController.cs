using HRManagementSystem.Data;
using HRManagementSystem.Models.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HRManagementSystem.Controllers
{
    [Authorize]
    public class JobTitlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobTitlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JobTitles
        public async Task<IActionResult> Index()
        {
            return View(await _context.JobTitles.AsNoTracking().ToListAsync());
        }

        // GET: JobTitles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var jobTitle = await _context.JobTitles
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id.Value);

            if (jobTitle == null) return NotFound();

            return View(jobTitle);
        }

        // GET: JobTitles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobTitles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NameAr,NameEn,DescriptionAr,DescriptionEn,IsActive")] JobTitle jobTitle)
        {
            if (ModelState.IsValid)
            {
                jobTitle.CreatedAt = DateTime.UtcNow;

                _context.JobTitles.Add(jobTitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobTitle);
        }

        // GET: JobTitles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var jobTitle = await _context.JobTitles.FindAsync(id.Value);
            if (jobTitle == null) return NotFound();
            return View(jobTitle);
        }

        // POST: JobTitles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,[Bind("Id,NameAr,NameEn,DescriptionAr,DescriptionEn,IsActive")]JobTitle jobTitle)
        {
            if (id != jobTitle.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(jobTitle);
            }

            var existingJobTitle = await _context.JobTitles.FindAsync(id);

            if (existingJobTitle is null)
            {
                return NotFound();
            }

            existingJobTitle.NameAr = jobTitle.NameAr;
            existingJobTitle.NameEn = jobTitle.NameEn;
            existingJobTitle.DescriptionAr = jobTitle.DescriptionAr;
            existingJobTitle.DescriptionEn = jobTitle.DescriptionEn;
            existingJobTitle.IsActive = jobTitle.IsActive;
            existingJobTitle.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: JobTitles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var jobTitle = await _context.JobTitles
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id.Value);
            if (jobTitle == null) return NotFound();

            return View(jobTitle);
        }

        // POST: JobTitles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobTitle = await _context.JobTitles.FindAsync(id);
            if (jobTitle != null)
            {
                _context.JobTitles.Remove(jobTitle);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool JobTitleExists(int id)
        {
            return _context.JobTitles.Any(e => e.Id == id);
        }
    }
}
