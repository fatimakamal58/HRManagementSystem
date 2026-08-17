using HRManagementSystem.Application.DTOs.Departments;
using HRManagementSystem.Application.DTOs.Shared;

namespace HRManagementSystem.Application.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync();
    Task<int> CreateAsync(CreateDepartmentDto dto);
    Task<DepartmentDto> GetByIdAsync(int id);
    Task UpdateAsync(UpdateDepartmentDto dto);
    Task DeleteAsync(int id);
    Task<List<LookupDto>> GetLookupAsync();
}