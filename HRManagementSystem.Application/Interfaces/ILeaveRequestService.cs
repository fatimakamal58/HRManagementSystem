using HRManagementSystem.Application.DTOs.LeaveBalances;
using HRManagementSystem.Application.DTOs.LeaveRequests;

namespace HRManagementSystem.Application.Interfaces;

public interface ILeaveRequestService
{
    Task CreateAsync(
    CreateLeaveRequestDto dto,
    string currentUserId);
    Task CancelAsync(
    int leaveRequestId,
    string currentUserId);
    Task ApproveAsync(
    int leaveRequestId,
    string approverUserId,
    string? comment);
    Task RejectAsync(
        int leaveRequestId,
        string approverUserId,
        string comment);
    Task<List<LeaveBalanceDto>> GetMyBalancesAsync(
    string currentUserId);
    Task<List<LeaveRequestDto>> GetMyRequestsAsync(
    string currentUserId);
    Task<List<PendingLeaveApprovalDto>> GetPendingApprovalsAsync(
    string currentUserId);

}