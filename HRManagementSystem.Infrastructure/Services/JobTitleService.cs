using HRManagementSystem.Application.DTOs.JobTitles;
using HRManagementSystem.Application.Exceptions;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities.HR;
using HRManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Infrastructure.Services;

public class JobTitleService : IJobTitleService
{
    private readonly ApplicationDbContext _context;

    public JobTitleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<JobTitleDto>> GetAllAsync()
    {
        return await _context.JobTitles
            .AsNoTracking()
            .OrderBy(jobTitle => jobTitle.NameAr)
            .Select(jobTitle => new JobTitleDto
            {
                Id = jobTitle.Id,
                NameAr = jobTitle.NameAr,
                NameEn = jobTitle.NameEn,
                DescriptionAr = jobTitle.DescriptionAr,
                DescriptionEn = jobTitle.DescriptionEn,
                IsActive = jobTitle.IsActive
            })
            .ToListAsync();
    }

    public async Task<JobTitleDto> GetByIdAsync(int id)
    {
        var jobTitle = await _context.JobTitles
            .AsNoTracking()
            .Where(jobTitle => jobTitle.Id == id)
            .Select(jobTitle => new JobTitleDto
            {
                Id = jobTitle.Id,
                NameAr = jobTitle.NameAr,
                NameEn = jobTitle.NameEn,
                DescriptionAr = jobTitle.DescriptionAr,
                DescriptionEn = jobTitle.DescriptionEn,
                IsActive = jobTitle.IsActive
            })
            .FirstOrDefaultAsync();

        if (jobTitle is null)
            throw new NotFoundException(nameof(JobTitle));

        return jobTitle;
    }

    public async Task<int> CreateAsync(CreateJobTitleDto dto)
    {
        var existingJobTitle = await _context.JobTitles
            .FirstOrDefaultAsync(jobTitle =>
                jobTitle.NameAr == dto.NameAr ||
                jobTitle.NameEn == dto.NameEn);

        if (existingJobTitle is not null)
        {
            if (existingJobTitle.IsActive)
                throw new DuplicateException(nameof(JobTitle));

            existingJobTitle.NameAr = dto.NameAr;
            existingJobTitle.NameEn = dto.NameEn;
            existingJobTitle.DescriptionAr = dto.DescriptionAr;
            existingJobTitle.DescriptionEn = dto.DescriptionEn;
            existingJobTitle.IsActive = true;
            existingJobTitle.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingJobTitle.Id;
        }

        var jobTitle = new JobTitle
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            DescriptionAr = dto.DescriptionAr,
            DescriptionEn = dto.DescriptionEn,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.JobTitles.Add(jobTitle);

        await _context.SaveChangesAsync();

        return jobTitle.Id;
    }

    public async Task UpdateAsync(UpdateJobTitleDto dto)
    {
        var jobTitle = await _context.JobTitles
            .FirstOrDefaultAsync(jobTitle => jobTitle.Id == dto.Id);

        if (jobTitle is null)
            throw new NotFoundException(nameof(JobTitle));

        var duplicateExists = await _context.JobTitles
            .AnyAsync(otherJobTitle =>
                otherJobTitle.Id != dto.Id &&
                (
                    otherJobTitle.NameAr == dto.NameAr ||
                    otherJobTitle.NameEn == dto.NameEn
                ));

        if (duplicateExists)
            throw new DuplicateException(nameof(JobTitle));

        jobTitle.NameAr = dto.NameAr;
        jobTitle.NameEn = dto.NameEn;
        jobTitle.DescriptionAr = dto.DescriptionAr;
        jobTitle.DescriptionEn = dto.DescriptionEn;
        jobTitle.IsActive = dto.IsActive;
        jobTitle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var jobTitle = await _context.JobTitles
            .FirstOrDefaultAsync(jobTitle => jobTitle.Id == id);

        if (jobTitle is null)
            throw new NotFoundException(nameof(JobTitle));

        jobTitle.IsActive = false;
        jobTitle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}