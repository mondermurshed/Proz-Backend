using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class SalaryScheduleConfiguration : IEntityTypeConfiguration<Salary_Schedule>
    {
        public void Configure(EntityTypeBuilder<Salary_Schedule> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(ed => ed.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(s => s.CurrentPeriodStartDate)
                .HasColumnType("date");

            builder.Property(s => s.CurrentPeriodEndDate)
                .HasColumnType("date");

            builder.Property(s => s.PaymentCounter);
            
            builder.OwnsOne(s => s.EmployeeBonus, bonus =>
            {
                bonus.Property(b => b.Amount).HasPrecision(18, 2);
                bonus.Property(b => b.Currency).HasMaxLength(4).IsUnicode().IsRequired();
            });

            builder.OwnsOne(s => s.Employee_Deduction, deduction =>
            {
                deduction.Property(d => d.Amount).HasPrecision(18, 2);
                deduction.Property(d => d.Currency).HasMaxLength(4).IsUnicode().IsRequired();
            });

            builder.Property(s => s.Version)
                .IsRowVersion();

            builder.HasOne(s => s.EmployeeDepartmentNA)
                .WithOne(e => e.SalaryScheduleNA)
                .HasForeignKey<Salary_Schedule>(s => s.EmployeeDepartment_FK);
        }
    }
}
