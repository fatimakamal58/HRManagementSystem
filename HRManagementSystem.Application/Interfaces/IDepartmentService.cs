using HRManagementSystem.Application.DTOs.Departments;

namespace HRManagementSystem.Application.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync();
    Task<int> CreateAsync(CreateDepartmentDto dto);
    Task<DepartmentDto> GetByIdAsync(int id);
    Task UpdateAsync(UpdateDepartmentDto dto);
    Task DeleteAsync(int id);
}