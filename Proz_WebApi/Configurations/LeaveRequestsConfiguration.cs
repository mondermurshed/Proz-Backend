using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class LeaveRequestsConfiguration : IEntityTypeConfiguration<LeaveRequests>
    {
        public void Configure(EntityTypeBuilder<LeaveRequests> builder)
        {
            builder.HasKey(l => l.Id);
            builder.Property(ed => ed.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(l => l.StartDate)
                .HasColumnType("date");

            builder.Property(l => l.EndDate)
                .HasColumnType("date");

            builder.Property(l => l.Reason)
                .HasMaxLength(500)
                .IsUnicode()
                .IsRequired();

            builder.Property(l => l.IsEditable);

            builder.Property(l => l.LastUpdate)
                .HasColumnType("datetime2");

            builder.Property(l => l.DMStatus)
                .HasMaxLength(15)
                .IsUnicode();

            builder.Property(l => l.FinalStatus)
                .HasMaxLength(15)
                .IsUnicode();

            builder.Property(l => l.Created_At)
                .HasColumnType("datetime2");

            builder.Property(l => l.DepartmentManagerComment)
                .HasMaxLength(250)
                .IsUnicode();

            builder.Property(l => l.FinalStatus_Comment)
                .HasMaxLength(250)
                .IsUnicode();

            builder.Property(l => l.Decision_At)
                .HasColumnType("datetime2");

            builder.Property(l => l.Version)
                .IsRowVersion();

            builder.HasOne(l => l.EmployeeNA)
                .WithMany(e => e.LeaveRequestsNA)
                .HasForeignKey(l => l.Requester_Employee_FK);

            builder.HasOne(l => l.DepartmentManagerNA)
                .WithMany(e => e.DepartmentManagerLeaveRequestsDealsNA)
                .HasForeignKey(l => l.DepartmentManager_FK)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.HandlerEmployeeNA)
                .WithMany(e => e.ADHRLeaveRequestsDealsNA)
                .HasForeignKey(l => l.HandlerEmployee_FK)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
