using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class ExtendedIdentityUserRolesConfiguration : IEntityTypeConfiguration<ExtendedIdentityUserRolesDesktop>
    {
        public void Configure(EntityTypeBuilder<ExtendedIdentityUserRolesDesktop> builder)
        {
            builder.HasOne(ur => ur.UserNA)
            .WithMany(u => u.UserRolesNA)
            .HasForeignKey(u => u.UserId)
            .IsRequired();
          
            builder.HasOne(ur => ur.RoleNA)
           .WithMany(r => r.UserRolesNA)
           .HasForeignKey(ur => ur.RoleId)
           .IsRequired();

        }
            
    }
}
