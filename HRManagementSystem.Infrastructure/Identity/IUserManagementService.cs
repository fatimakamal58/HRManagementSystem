using HRManagementSystem.Infrastructure.Identity.DTOs;

namespace HRManagementSystem.Infrastructure.Identity;

public interface IUserManagementService
{
    Task CreateAsync(CreateUserDto dto);
}