using HRManagementSystem.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagementSystem.Infrastructure.Data.Configurations;

public class LeaveBalanceConfiguration
    : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.Property(x => x.AllocatedDays)
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.CarryForwardDays)
            .HasColumnType("decimal(5,2)");

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.LeaveBalances)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LeaveType)
            .WithMany(x => x.LeaveBalances)
            .HasForeignKey(x => x.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
            {
                x.EmployeeId,
                x.LeaveTypeId,
                x.Year
            })
            .IsUnique();
    }
}