using HRManagementSystem.Application.DTOs.Departments;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Application.Exceptions;

namespace HRManagementSystem.Infrastructure.Services;

public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _context;

    public DepartmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartmentDto>> GetAllAsync()
    {
        return await _context.Departments
            .AsNoTracking()
            .Select(department => new DepartmentDto
            {
                Id = department.Id,
                NameAr = department.NameAr,
                NameEn = department.NameEn,
                DescriptionAr = department.DescriptionAr,
                DescriptionEn = department.DescriptionEn,
                IsActive = department.IsActive
            })
            .ToListAsync();
    }

    public async Task<int> CreateAsync(CreateDepartmentDto dto)
    {
        var existingDepartment = await _context.Departments
            .FirstOrDefaultAsync(d =>
                d.NameAr == dto.NameAr ||
                d.NameEn == dto.NameEn);

        if (existingDepartment != null)
        {
            if (existingDepartment.IsActive)
                throw new DuplicateException(nameof(Department));

            existingDepartment.IsActive = true;
            existingDepartment.NameAr = dto.NameAr;
            existingDepartment.NameEn = dto.NameEn;
            existingDepartment.DescriptionAr = dto.DescriptionAr;
            existingDepartment.DescriptionEn = dto.DescriptionEn;
            existingDepartment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingDepartment.Id;
        }

        var department = new Department
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            DescriptionAr = dto.DescriptionAr,
            DescriptionEn = dto.DescriptionEn,
            IsActive = dto.IsActive
        };

        _context.Departments.Add(department);

        await _context.SaveChangesAsync();

        return department.Id;
    }
    public async Task<DepartmentDto> GetByIdAsync(int id)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                NameAr = d.NameAr,
                NameEn = d.NameEn,
                DescriptionAr = d.DescriptionAr,
                DescriptionEn = d.DescriptionEn,
                IsActive = d.IsActive
            })
            .FirstOrDefaultAsync();

        if (department == null)
            throw new NotFoundException(nameof(Department));

        return department;
    }
    public async Task UpdateAsync(UpdateDepartmentDto dto)
    {
        var department = await _context.Departments.FindAsync(dto.Id);

        if (department == null)
            throw new NotFoundException(nameof(Department));
       
        var exists = await _context.Departments
          .AnyAsync(d =>
            d.Id != dto.Id &&
            (
                d.NameAr == dto.NameAr ||
                d.NameEn == dto.NameEn
          ));
        if (exists)
            throw new DuplicateException(nameof(Department));

        department.NameAr = dto.NameAr;
        department.NameEn = dto.NameEn;
        department.DescriptionAr = dto.DescriptionAr;
        department.DescriptionEn = dto.DescriptionEn;
        department.IsActive = dto.IsActive;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
            throw new NotFoundException(nameof(Department));

        _context.Departments.Remove(department);

        await _context.SaveChangesAsync();
    }
}