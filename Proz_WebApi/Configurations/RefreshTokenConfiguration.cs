using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenDesktop>
    {
        public void Configure(EntityTypeBuilder<RefreshTokenDesktop> builder)
        {
         
   builder.HasOne(rt => rt.UserNA)         
   .WithMany(u=>u.RefreshTokensNA)                     
   .HasForeignKey(rt => rt.UserFK) 
   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
