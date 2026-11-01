using HRManagementSystem.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagementSystem.Infrastructure.Data.Configurations;

public class WorkWeekSettingConfiguration
    : IEntityTypeConfiguration<WorkWeekSetting>
{
    public void Configure(EntityTypeBuilder<WorkWeekSetting> builder)
    {
        builder.HasIndex(x => x.DayOfWeek)
            .IsUnique();
    }
}