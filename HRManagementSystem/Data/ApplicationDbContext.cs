using HRManagementSystem.Models.HR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Department> Departments { get; set; } = default!;
        public DbSet<JobTitle> JobTitles { get; set; } = default!;
    }
}
