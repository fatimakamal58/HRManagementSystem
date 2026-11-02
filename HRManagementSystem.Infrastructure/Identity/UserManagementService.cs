using HRManagementSystem.Infrastructure.Identity.DTOs;
using Microsoft.AspNetCore.Identity;

namespace HRManagementSystem.Infrastructure.Identity;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task CreateAsync(CreateUserDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var role = dto.Role.Trim();

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
            throw new InvalidOperationException(
                "A user with this email already exists.");

        if (!await _roleManager.RoleExistsAsync(role))
            throw new InvalidOperationException(
                "The selected role does not exist.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult =
            await _userManager.CreateAsync(user, dto.Password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                createResult.Errors.Select(x => x.Description));

            throw new InvalidOperationException(errors);
        }

        var roleResult =
            await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            // ما نخلي مستخدم بدون Role إذا فشلت العملية
            await _userManager.DeleteAsync(user);

            var errors = string.Join(
                ", ",
                roleResult.Errors.Select(x => x.Description));

            throw new InvalidOperationException(
                $"Could not assign role: {errors}");
        }
    }
}