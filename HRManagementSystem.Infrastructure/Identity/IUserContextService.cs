

using HRManagementSystem.Infrastructure.Identity.DTOs;

namespace HRManagementSystem.Infrastructure.Identity
{
    public interface IUserContextService
    {
        Task<int?> GetEmployeeIdAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string role);

        Task<List<UserLookupDto>> GetAvailableUsersAsync(
            string? currentUserId = null);
    }
}
