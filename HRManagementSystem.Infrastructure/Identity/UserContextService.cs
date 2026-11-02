using HRManagementSystem.Infrastructure.Data;
using HRManagementSystem.Infrastructure.Identity.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Infrastructure.Identity;

public class UserContextService : IUserContextService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserContextService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<int?> GetEmployeeIdAsync(string userId)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<List<UserLookupDto>> GetAvailableUsersAsync(
    string? currentUserId = null)
    {
        var linkedUserIds = await _context.Employees
            .AsNoTracking()
            .Where(x =>
                x.UserId != null &&
                x.UserId != currentUserId)
            .Select(x => x.UserId!)
            .ToListAsync();

        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

        var adminUserIds = adminUsers
            .Select(x => x.Id)
            .ToList();

        return await _userManager.Users
            .AsNoTracking()
            .Where(x =>
                !linkedUserIds.Contains(x.Id) &&
                !adminUserIds.Contains(x.Id))
            .OrderBy(x => x.Email)
            .Select(x => new UserLookupDto
            {
                Id = x.Id,
                Email = x.Email ?? x.UserName ?? x.Id
            })
            .ToListAsync();
    }
}