using HRManagementSystem.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagementSystem.Infrastructure.Data.Configurations;

public class LeaveApprovalConfiguration
    : IEntityTypeConfiguration<LeaveApproval>
{
    public void Configure(EntityTypeBuilder<LeaveApproval> builder)
    {
        builder.Property(x => x.ApproverUserId)
            .HasMaxLength(450);

        builder.Property(x => x.Comment)
            .HasMaxLength(500);

        builder.HasOne(x => x.LeaveRequest)
            .WithMany(x => x.Approvals)
            .HasForeignKey(x => x.LeaveRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
            {
                x.LeaveRequestId,
                x.StepOrder
            })
            .IsUnique();
    }
}