using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Attendance_RecorderConfiguration : IEntityTypeConfiguration<Attendance_Recorder>
    {
        public void Configure(EntityTypeBuilder<Attendance_Recorder> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.CheckInTime)
                .HasColumnType("time");

            builder.Property(a => a.CheckOutTime)
                .HasColumnType("time");

            builder.Property(a => a.CheckInStatus)
                .HasMaxLength(20)
                .IsUnicode()
                .IsRequired();

            builder.Property(a => a.CheckOutStatus)
                .HasMaxLength(20)
                .IsUnicode();

            builder.Property(a => a.CheckInComment)
                .HasMaxLength(350)
                .IsUnicode();

            builder.Property(a => a.CheckOutComment)
                .HasMaxLength(350)
                .IsUnicode();

            builder.HasOne(a => a.EmployeeDepartmentNA)
                .WithMany(ed => ed.AttendanceRecordersNA)
                .HasForeignKey(a => a.EmployeeDepartment_FK);
        }
    }
}
