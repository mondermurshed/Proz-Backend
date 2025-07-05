using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class DepartmentContactMethodsConfiguration : IEntityTypeConfiguration<DepartmentContactMethods>
    {
        public void Configure(EntityTypeBuilder<DepartmentContactMethods> builder)
        {
            builder.HasKey(dc => dc.Id);

            builder.Property(dc => dc.ContactMethod)
                .HasMaxLength(20)
                .IsUnicode()
                .IsRequired();

            builder.Property(dc => dc.ContactDetail)
                .HasMaxLength(320)
                .IsUnicode()
                .IsRequired();

            builder.Property(dc => dc.Purpose)
                .HasMaxLength(20)
                .IsUnicode()
                .IsRequired();

            builder.HasOne(dc => dc.Department)
                .WithMany(d => d.DepartmentContactMethodsNA)
                .HasForeignKey(dc => dc.Department_FK);
        }
    }
}
