using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Payment_RecordsConfigurationcs : IEntityTypeConfiguration<Payment_Records>
    {
        public void Configure(EntityTypeBuilder<Payment_Records> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(ed => ed.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(p => p.PaymentDateCreatedAt)
                .HasColumnType("date");

            builder.Property(p => p.PaymentPeriodStart)
                .HasColumnType("date");

            builder.Property(p => p.PaymentPeriodEnd)
                .HasColumnType("date");

            builder.Property(p => p.PaymentCounter);

            builder.Property(p => p.Salary)
                .HasPrecision(18, 2);

        
            builder.Property(p => p.FixedBonus)
                .HasPrecision(18, 2);

          

            builder.Property(p => p.FixedBonusNote)
                .HasMaxLength(150)
                .IsUnicode();

            builder.Property(p => p.PerformanceBonus)
                .HasPrecision(18, 2);

          

            builder.Property(p => p.PerformanceBonusNote)
                .HasMaxLength(150)
                .IsUnicode();

            builder.Property(p => p.Deduction)
                .HasPrecision(18, 2);

         

            builder.Property(p => p.DeductionNote)
                .HasMaxLength(150)
                .IsUnicode();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.Version)
                .IsRowVersion();

            builder.HasOne(p => p.EmployeeDepartmentsNA)
                .WithMany(ed => ed.PaymentRecordsNA)
                .HasForeignKey(p => p.EmployeeDepartments_FK);
        }
    }
}
