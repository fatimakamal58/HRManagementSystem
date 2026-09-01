using HRManagementSystem.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagementSystem.Infrastructure.Data.Configurations;

public class DepartmentConfiguration
    : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.Property(d => d.NameAr)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.NameEn)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.DescriptionAr)
            .HasMaxLength(300);

        builder.Property(d => d.DescriptionEn)
            .HasMaxLength(300);
    }
}