using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class ExtendedIdentityRolesConfiguration : IEntityTypeConfiguration<ExtendedIdentityRolesDesktop>
    {
        public void Configure(EntityTypeBuilder<ExtendedIdentityRolesDesktop> builder)
        {

            builder.Property(p => p.RoleColorCode)
            .HasMaxLength(20).IsRequired().IsUnicode();

            builder.HasMany(r => r.UserRolesNA)
                     .WithOne(ur => ur.RoleNA)
                     .HasForeignKey(ur => ur.RoleId);


        }
    }
}
