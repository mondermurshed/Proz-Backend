using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class EmployeeSalaryHistoryConfiguration : IEntityTypeConfiguration<EmployeeSalaryHistory>
    {
        public void Configure(EntityTypeBuilder<EmployeeSalaryHistory> builder)
        {
            builder.HasKey(eh => eh.Id);

            builder.Property(eh => eh.Salary)
                .HasPrecision(18, 2);

            builder.Property(eh => eh.EffectiveFrom)
                .HasColumnType("date");

            builder.Property(eh => eh.EffectiveTo)
                .HasColumnType("date");

            builder.HasOne(eh => eh.EmployeeDepartmentsNA)
                .WithMany(ed => ed.EmployeeSalaryHistoryNA)
                .HasForeignKey(eh => eh.EmployeeDepartments_FK);
        }
    }
}
