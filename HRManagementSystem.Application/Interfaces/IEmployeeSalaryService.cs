using HRManagementSystem.Application.DTOs.EmployeeSalaries;

namespace HRManagementSystem.Application.Interfaces
{
    public interface IEmployeeSalaryService
    {
        Task CreateInitialSalaryAsync(CreateInitialSalaryDto dto);

        Task ChangeSalaryAsync(ChangeEmployeeSalaryDto dto);

        Task<EmployeeSalaryDto?> GetCurrentSalaryAsync(int employeeId);

        Task<List<EmployeeSalaryDto>> GetSalaryHistoryAsync(int employeeId);
    }
}
