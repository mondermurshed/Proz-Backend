using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class EmployeesConfiguration : IEntityTypeConfiguration<Employees>
    {
        public void Configure(EntityTypeBuilder<Employees> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(ed => ed.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode()
                .IsRequired();

            builder.HasOne(e => e.IdentityUserNA)
                .WithOne(iu => iu.EmployeesNA)
                .HasForeignKey<Employees>(e => e.IdentityUsers_FK)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.EmployeeToDepatment)
            .WithOne(ed => ed.EmployeeNA)
                .HasForeignKey(ed => ed.Employee_FK);
              

           


            builder.HasMany(e => e.FeedbackAnswerNA)
                .WithOne(fa => fa.EmployeeNA)
                .HasForeignKey(fa => fa.RespondentAccount_FK);

            builder.HasMany(e => e.LogsNA)
                .WithOne(a => a.TargetEntityNA)
                .HasForeignKey(a => a.TargetEntity_FK);


            builder.HasMany(e => e.PerformedLogsNA)
             .WithOne(a => a.PerformerAccountNA)
             .HasForeignKey(a => a.PerformerAccount_FK);

            builder.HasMany(e => e.ManagerAtNA)
                .WithOne(d => d.ManagerNA)
                .HasForeignKey(d => d.Manager_FK)
                .OnDelete(DeleteBehavior.Restrict);

         

            builder.HasMany(e => e.PerformanceRecorderNA)
                .WithOne(p => p.ReviewerNA)
                .HasForeignKey(p => p.Reviewer_FK);


            builder.HasMany(e => e.DepartmentManagerLeaveRequestsDealsNA)
                .WithOne(l => l.DepartmentManagerNA)
                .HasForeignKey(l => l.DepartmentManager_FK);

            builder.HasMany(e => e.ADHRLeaveRequestsDealsNA)
                .WithOne(l => l.HandlerEmployeeNA)
                .HasForeignKey(l => l.HandlerEmployee_FK);

            builder.HasMany(e => e.NotificationsNA)
                .WithOne(n => n.EmployeeNA)
                .HasForeignKey(n => n.Target_FK);
        }
    }
}
