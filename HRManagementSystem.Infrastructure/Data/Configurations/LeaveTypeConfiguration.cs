using HRManagementSystem.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagementSystem.Infrastructure.Data.Configurations;

public class LeaveTypeConfiguration
    : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.Property(x => x.NameAr)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.NameEn)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DefaultDaysPerYear)
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.MaxCarryForwardDays)
            .HasColumnType("decimal(5,2)");
    }
}