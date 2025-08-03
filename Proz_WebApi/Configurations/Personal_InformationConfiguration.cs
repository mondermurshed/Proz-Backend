using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Personal_InformationConfiguration : IEntityTypeConfiguration<Personal_Information>
    {
        public void Configure(EntityTypeBuilder<Personal_Information> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(ed => ed.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(p => p.FullName)
                .HasMaxLength(150)
                .IsUnicode()
                .IsRequired();

            builder.Property(p => p.Gender)
                .HasMaxLength(6)
                .IsUnicode();
        

            builder.Property(p => p.Nationality)
                .HasMaxLength(50)
                .IsUnicode();

            builder.HasOne(p => p.IdentityUserNA)
            .WithOne(i => i.PersonalInformationNA)
            .HasForeignKey<Personal_Information>(p => p.IdentityUser_FK)
            .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
