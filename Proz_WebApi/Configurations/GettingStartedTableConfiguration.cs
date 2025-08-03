using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class GettingStartedTableConfiguration : IEntityTypeConfiguration<GettingStartedTable>
    {
        public void Configure(EntityTypeBuilder<GettingStartedTable> builder)
        {
            builder.HasKey(l => l.Id);
            builder.Property(ed => ed.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(l => l.SystemFirstRun)
         .HasColumnType("date");

            builder.Property(l => l.CompanyName)
               .HasMaxLength(100)
               .IsUnicode();

            builder.Property(l => l.CurrenyType)
            .HasMaxLength(3)
            .IsUnicode().IsRequired();


            builder.Property(l => l.PaymentFrquency)
            .HasMaxLength(30)
            .IsUnicode().IsRequired();

            builder.HasOne(u => u.AdminNA)
           .WithOne(e => e.GettingStartedTableNA)
           .HasForeignKey<GettingStartedTable>(e => e.Admin_FK);
        }
        }
}
