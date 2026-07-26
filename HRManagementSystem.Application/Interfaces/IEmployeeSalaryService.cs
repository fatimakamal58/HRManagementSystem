using HRManagementSystem.Application.DTOs.EmployeeSalaries;

namespace HRManagementSystem.Application.Interfaces
{
    public interface IEmployeeSalaryService
    {
        Task<List<EmployeeSalaryDto>> GetAllAsync();

        Task<EmployeeSalaryDto> GetByIdAsync(int id);

        Task CreateAsync(CreateEmployeeSalaryDto dto);

        Task UpdateAsync(UpdateEmployeeSalaryDto dto);

        Task DeleteAsync(int id);
    }
}
