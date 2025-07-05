using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Health_InformationConfiguration : IEntityTypeConfiguration<Health_Information>
    {
        public void Configure(EntityTypeBuilder<Health_Information> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.MedicalConditions)
                .HasMaxLength(500)
                .IsUnicode();

            builder.Property(h => h.Allergies)
                .HasMaxLength(500)
                .IsUnicode();

            builder.Property(h => h.EmergencyContactName)
                .HasMaxLength(50)
                .IsUnicode()
                .IsRequired();

            builder.Property(h => h.EmergencyContactPhone)
                .HasMaxLength(15)
                .IsUnicode()
                .IsRequired();

            builder.Property(h => h.CountryCodeOfThePhone)
                .HasMaxLength(5)
                .IsUnicode()
                .IsRequired();
            builder.HasOne(hi => hi.PersonalInformationNA)
            .WithOne(p => p.HealthInformationNA)
            .HasForeignKey<Health_Information>(hi => hi.PersonalInformation_FK);
        }
    }
}
