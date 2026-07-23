using HRManagementSystem.Application.DTOs.Departments;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
                IsActive = department.IsActive
            })
            .ToListAsync();
    }
}