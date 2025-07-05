using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Breaks_TimeConfiguration : IEntityTypeConfiguration<Breaks_Time>
    {
        public void Configure(EntityTypeBuilder<Breaks_Time> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.BreakStart)
                .HasColumnType("time");

            builder.Property(b => b.BreakEnd)
                .HasColumnType("time");

            builder.Property(b => b.BreakType)
                .HasMaxLength(35)
                .IsUnicode()
                .IsRequired();

            builder.Property(b => b.Notes)
                .HasMaxLength(350)
                .IsUnicode();

            builder.HasOne(b => b.ShiftNA)
                .WithMany(s => s.BreaksTimeNA)
                .HasForeignKey(b => b.Shift_FK);
        }
    }
}
