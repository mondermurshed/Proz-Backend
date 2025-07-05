using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class NotificationsConfiguration : IEntityTypeConfiguration<Notifications>
    {
        public void Configure(EntityTypeBuilder<Notifications> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .HasMaxLength(50)
                .IsUnicode()
                .IsRequired();

            builder.Property(n => n.Message)
                .HasMaxLength(500)
                .IsUnicode()
                .IsRequired();

            builder.Property(n => n.Created_At)
                .HasColumnType("datetime2");

            builder.Property(n => n.Seen_At)
                .HasColumnType("datetime2");

            builder.Property(n => n.Type)
                .HasMaxLength(25)
                .IsUnicode()
                .IsRequired();

            builder.Property(n => n.Priority)
                .HasMaxLength(6)
                .IsUnicode()
                .IsRequired();

            builder.Property(n => n.IsArchived)
                .IsRequired();

            builder.HasOne(n => n.EmployeeNA)
                .WithMany(e => e.NotificationsNA)
                .HasForeignKey(n => n.Target_FK);
        }
    }
}
