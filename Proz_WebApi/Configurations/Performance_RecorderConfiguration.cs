using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Performance_RecorderConfiguration : IEntityTypeConfiguration<Performance_Recorder>
    {
        public void Configure(EntityTypeBuilder<Performance_Recorder> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PerformanceRating);

            builder.Property(p => p.ReviewerComment)
                .HasMaxLength(125)
                .IsUnicode();

            builder.Property(p => p.CreatedAt)
                .HasColumnType("datetime2");

            builder.HasOne(p => p.EmployeeDepartmentNA)
                .WithMany(ed => ed.PerformanceRecordersNA)
                .HasForeignKey(p => p.EmployeeDepartment_FK);
            
            builder.HasOne(p => p.ReviewerNA)
                .WithMany(e => e.PerformanceRecorderNA)
                .HasForeignKey(p => p.Reviewer_FK)
                .OnDelete(DeleteBehavior.Restrict); // prevent deleting reviewer from deleting record
        }
    }
}
