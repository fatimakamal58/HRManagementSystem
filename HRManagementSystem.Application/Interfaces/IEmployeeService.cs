using HRManagementSystem.Application.DTOs.Employees;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetAllAsync();

        Task<EmployeeDto> GetByIdAsync(int id);

        Task CreateAsync(CreateEmployeeDto dto);

        Task UpdateAsync(UpdateEmployeeDto dto);

        Task DeleteAsync(int id);
    }
}
