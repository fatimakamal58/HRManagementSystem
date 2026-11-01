using HRManagementSystem.Application.DTOs.LeaveRequests;

namespace HRManagementSystem.Application.Interfaces;

public interface ILeaveRequestService
{
    Task CreateAsync(CreateLeaveRequestDto dto);

    Task CancelAsync(int leaveRequestId);

    Task ApproveAsync(
    int leaveRequestId,
    string approverUserId,
    string? comment);

    Task RejectAsync(
        int leaveRequestId,
        string approverUserId,
        string comment);
}