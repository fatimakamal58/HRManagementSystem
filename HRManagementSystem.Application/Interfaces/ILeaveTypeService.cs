using HRManagementSystem.Application.DTOs.LeaveTypes;

namespace HRManagementSystem.Application.Interfaces;

public interface ILeaveTypeService
{
    Task CreateAsync(CreateLeaveTypeDto dto);
    Task<List<LeaveTypeDto>> GetAllAsync();
    Task<LeaveTypeDto?> GetByIdAsync(int id);
    Task AddApprovalStepAsync(CreateLeaveApprovalStepDto dto);
    Task<List<LeaveApprovalStepDto>> GetApprovalStepsAsync(
    int leaveTypeId);
    Task DeactivateApprovalStepAsync(int id);
    Task ActivateApprovalStepAsync(int id);
}