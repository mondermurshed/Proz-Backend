using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class DepartmentsConfigurationcs : IEntityTypeConfiguration<Departments>
    {
        public void Configure(EntityTypeBuilder<Departments> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(ed => ed.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(d => d.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode()
                .IsRequired();

            builder.Property(p => p.DepartmentDefaultSalary)
              .HasPrecision(18, 2);

            builder.HasOne(d => d.ManagerNA)
             .WithMany(e => e.ManagerAtNA)
             .HasForeignKey(d => d.Manager_FK)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.ParentDepartmentNA)
      .WithMany(p => p.SubDepartmentsNA)
      .HasForeignKey(d => d.ParentDepartment_FK)
      .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.DepartmentContactMethodsNA)
           .WithOne(dc => dc.Department)
           .HasForeignKey(dc => dc.Department_FK);
        }
    }
}
