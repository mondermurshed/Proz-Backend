using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class LeaveRequestsHigherRoleConfiguration : IEntityTypeConfiguration<LeaveRequestsHigherRole>
    {
        public void Configure(EntityTypeBuilder<LeaveRequestsHigherRole> builder)
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



            builder.Property(l => l.LastUpdate)
                .HasColumnType("datetime2");
          
            builder.Property(l => l.LastUpdateAdmin).HasColumnType("datetime2");


            builder.Property(l => l.RequesterStatus)
                .HasMaxLength(15)
                .IsUnicode();

     

            builder.Property(l => l.HRStatus)
                .HasMaxLength(15)
                .IsUnicode();


            builder.Property(l => l.HR_Comment)
                .HasMaxLength(250)
                .IsUnicode();

            builder.Property(l => l.Decision_At)
                .HasColumnType("datetime2");

            builder.Property(l => l.Version)
                .IsRowVersion();

            builder.HasOne(l => l.RequesterManagerNA)
                .WithMany(e => e.ManagerLeaveRequestsNA)
                .HasForeignKey(l => l.Requester_Employee_FK);

        

            builder.HasOne(l => l.HRManagerNA)
                .WithMany(e => e.HRManagerLeaveRequestsNA)
                .HasForeignKey(l => l.HREmployee_FK)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
