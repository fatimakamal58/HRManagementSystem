using HRManagementSystem.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagementSystem.Infrastructure.Data.Configurations;

public class JobTitleConfiguration
    : IEntityTypeConfiguration<JobTitle>
{
    public void Configure(EntityTypeBuilder<JobTitle> builder)
    {
        builder.Property(j => j.NameAr)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.NameEn)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.DescriptionAr)
            .HasMaxLength(500);

        builder.Property(j => j.DescriptionEn)
            .HasMaxLength(500);
    }
}