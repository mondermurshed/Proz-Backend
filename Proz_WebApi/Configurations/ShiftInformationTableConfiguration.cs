using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class ShiftInformationTableConfiguration : IEntityTypeConfiguration<ShiftInformationTable>
    {
        public void Configure(EntityTypeBuilder<ShiftInformationTable> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Shift_Starts)
                .HasColumnType("time");

            builder.Property(s => s.Shift_Ends)
                .HasColumnType("time");

            builder.Property(s => s.TotalHours);

            builder.Property(s => s.ShiftType)
                .HasMaxLength(15)
                .IsUnicode()
                .IsRequired();

            builder.HasMany(s => s.BreaksTimeNA)
                .WithOne(b => b.ShiftNA)
                .HasForeignKey(b => b.Shift_FK);

            builder.HasMany(s => s.EmployeeDepartmentsNA)
                .WithOne(ed => ed.ShiftNA)
                .HasForeignKey(ed => ed.Shift_FK);

        }
    }
}
