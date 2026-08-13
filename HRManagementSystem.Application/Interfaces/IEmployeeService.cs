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
        Task<EmployeeFormDto> GetCreateFormAsync();

        Task CreateAsync(CreateEmployeeDto dto);

        Task<UpdateEmployeeDto> GetForUpdateAsync(int id);
        Task<UpdateEmployeeFormDto> GetEditViewModelAsync(int id);
        Task UpdateAsync(UpdateEmployeeDto dto);

        Task DeleteAsync(int id);
        Task<List<EmployeeDto>> GetDeletedAsync();
        Task RestoreAsync(int id);
    }
}
