using HRManagementSystem.Application.DTOs.JobTitles;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Controllers
{
    [Authorize]
    public class JobTitlesController : Controller
    {
        private readonly IJobTitleService _jobTitleService;

        public JobTitlesController(IJobTitleService jobTitleService)
        {
            _jobTitleService = jobTitleService;
        }

        // GET: JobTitles
        public async Task<IActionResult> Index()
        {
            var jobTitles = await _jobTitleService.GetAllAsync();

            return View(jobTitles);
        }

        // GET: JobTitles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var jobTitle = await _jobTitleService.GetByIdAsync(id);

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
        public async Task<IActionResult> Create(CreateJobTitleDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _jobTitleService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        // GET: JobTitles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var jobTitle = await _jobTitleService.GetByIdAsync(id);

            var model = new UpdateJobTitleDto
            {
                Id = jobTitle.Id,
                NameAr = jobTitle.NameAr,
                NameEn = jobTitle.NameEn,
                DescriptionAr = jobTitle.DescriptionAr,
                DescriptionEn = jobTitle.DescriptionEn,
                IsActive = jobTitle.IsActive
            };

            return View(model);
        }

        // POST: JobTitles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateJobTitleDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _jobTitleService.UpdateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        // GET: JobTitles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var jobTitle = await _jobTitleService.GetByIdAsync(id);

            return View(jobTitle);
        }

        // POST: JobTitles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _jobTitleService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }


    }
}
