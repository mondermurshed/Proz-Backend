using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class CurrentAddressConfiguration : IEntityTypeConfiguration<CurrentAddress>
    {
        public void Configure(EntityTypeBuilder<CurrentAddress> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CountryName)
                .HasMaxLength(50)
                .IsUnicode()
                .IsRequired();

            builder.Property(c => c.CityName)
                .HasMaxLength(50)
                .IsUnicode()
                .IsRequired();

            builder.Property(c => c.StreetAddress)
                .HasMaxLength(50)
                .IsUnicode()
                .IsRequired();

            builder.Property(c => c.DepartmentNumber)
                .HasMaxLength(50)
                .IsUnicode();

            builder.Property(c => c.Describe_The_Location)
                .HasMaxLength(500)
                .IsUnicode()
                .IsRequired();

            builder.HasOne(ca => ca.PersonalInformationNA)
            .WithOne(p => p.CurrentAddressNA)
            .HasForeignKey<CurrentAddress>(ca => ca.PersonalInformation_FK);
        }
    }
}
