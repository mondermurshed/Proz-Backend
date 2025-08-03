using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class LoginHistoryConfigurationcs : IEntityTypeConfiguration<LoginHistory>
    {
        public void Configure(EntityTypeBuilder<LoginHistory> builder)
        {
            builder.HasKey(l => l.ID);

            builder.Property(ed => ed.ID).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(l => l.LoggedAt)
                .HasColumnType("datetime2");

        

            builder.HasOne(l => l.ExtendedIdentityUsersDesktopNA)
                .WithMany(u => u.LoginHistroyNA)
                .HasForeignKey(l => l.ExtendedIdentityUsersDesktop_FK);
        }
    }
}
