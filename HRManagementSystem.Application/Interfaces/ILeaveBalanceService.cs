using HRManagementSystem.Application.DTOs.LeaveBalances;

namespace HRManagementSystem.Application.Interfaces;

public interface ILeaveBalanceService
{
    Task<List<LeaveBalanceDto>> GetAllAsync();
    Task SetBalanceAsync(SetLeaveBalanceDto dto);
    Task InitializeYearBalancesAsync(
    InitializeLeaveBalanceDto dto);
    Task<InitializeLeaveBalanceDto> GetInitializationFormAsync(int year);
    Task<UpdateLeaveBalanceDto?> GetForUpdateAsync(int id);

    Task UpdateAsync(UpdateLeaveBalanceDto dto);

    Task<List<LeaveBalanceDto>> GetMyBalancesAsync(
    string currentUserId);
}