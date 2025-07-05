using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class ExtendedIdentityUsersConfiguration : IEntityTypeConfiguration<ExtendedIdentityUsersDesktop>
    {
        public void Configure(EntityTypeBuilder<ExtendedIdentityUsersDesktop> builder)
        { 
            builder.HasKey(u=>u.Id);
            builder.Property(u => u.LastOnline)
                .HasColumnType("datetime2");

            builder.Property(u => u.Status)
                .HasMaxLength(15)
                .IsUnicode()
                .IsRequired();

            builder.HasMany(u => u.UserRolesNA) 
          .WithOne(ur => ur.UserNA)           
          .HasForeignKey(ur => ur.UserId) //in Identity package they name the FK by ID. 
          .IsRequired();

            builder.HasMany(u => u.RefreshTokensNA)
            .WithOne(rt => rt.UserNA)
            .HasForeignKey(rt => rt.UserFK);

            builder.HasOne(u => u.PersonalInformationNA)
            .WithOne(p => p.IdentityUserNA)
            .HasForeignKey<Personal_Information>(p => p.IdentityUser_FK);

            builder.HasMany(u => u.LoginHistroyNA)
            .WithOne(lh => lh.ExtendedIdentityUsersDesktopNA)
            .HasForeignKey(lh => lh.ExtendedIdentityUsersDesktop_FK);

            builder.HasOne(u => u.EmployeesNA)
            .WithOne(e => e.IdentityUserNA)
            .HasForeignKey<Employees>(e => e.IdentityUsers_FK);
        }
    }
}
