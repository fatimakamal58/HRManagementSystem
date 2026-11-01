using HRManagementSystem.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagementSystem.Infrastructure.Data.Configurations;

public class LeaveApprovalStepConfiguration
    : IEntityTypeConfiguration<LeaveApprovalStep>
{
    public void Configure(EntityTypeBuilder<LeaveApprovalStep> builder)
    {
        builder.HasOne(x => x.LeaveType)
            .WithMany(x => x.ApprovalSteps)
            .HasForeignKey(x => x.LeaveTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
            {
                x.LeaveTypeId,
                x.StepOrder
            })
            .IsUnique();
    }
}