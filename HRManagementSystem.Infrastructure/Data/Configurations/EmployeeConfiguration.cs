using HRManagementSystem.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagementSystem.Infrastructure.Data.Configurations;

public class EmployeeConfiguration
    : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.Property(e => e.EmployeeNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(e => e.FirstNameAr)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.SecondNameAr)
            .HasMaxLength(50);

        builder.Property(e => e.ThirdNameAr)
            .HasMaxLength(50);

        builder.Property(e => e.LastNameAr)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.FirstNameEn)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.SecondNameEn)
            .HasMaxLength(50);

        builder.Property(e => e.ThirdNameEn)
            .HasMaxLength(50);

        builder.Property(e => e.LastNameEn)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.NationalId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(e => e.EmployeeNumber)
            .IsUnique();

        builder.HasIndex(e => e.NationalId)
            .IsUnique();

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.JobTitle)
            .WithMany(j => j.Employees)
            .HasForeignKey(e => e.JobTitleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Manager)
            .WithMany(e => e.DirectReports)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}