using HRManagementSystem.Application.DTOs.Departments;

namespace HRManagementSystem.Application.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync();
}