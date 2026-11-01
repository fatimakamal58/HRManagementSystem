using HRManagementSystem.Domain.Entities.HR;
using Microsoft.AspNetCore.Identity;

namespace HRManagementSystem.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public Employee? Employee { get; set; }
}