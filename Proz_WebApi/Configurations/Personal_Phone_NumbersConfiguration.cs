using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Personal_Phone_NumbersConfiguration : IEntityTypeConfiguration<Personal_Phone_Numbers>
    {
        public void Configure(EntityTypeBuilder<Personal_Phone_Numbers> builder)
        {
        builder.HasKey(p => p.Id);
            builder.Property(p => p.CountryNumber)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(5);
            
            builder.Property(p=>p.Number)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(15);

            builder.Property(p=>p.NumberType)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength (15);

            builder.HasOne(pn => pn.PersonalInformationNA)
            .WithMany(p => p.PhoneNumbersNA)
            .HasForeignKey(pn => pn.PersonalInformation_FK);
        }
    }
}
