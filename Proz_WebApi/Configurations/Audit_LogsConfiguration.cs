using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Audit_LogsConfiguration : IEntityTypeConfiguration<Audit_Logs>
    {
        public void Configure(EntityTypeBuilder<Audit_Logs> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.ActionType)
                .HasMaxLength(25)
                .IsUnicode()
                .IsRequired();

            builder.Property(a => a.Performed_At)
                .HasColumnType("datetime2");

            builder.HasOne(a=>a.TargetEntityNA)
                .WithMany(e => e.LogsNA)
                .HasForeignKey(a => a.TargetEntity_FK)
                .OnDelete(DeleteBehavior.SetNull);


                     builder.HasOne(a => a.PerformerAccountNA)
             .WithMany(e => e.PerformedLogsNA)
             .HasForeignKey(a => a.PerformerAccount_FK)
             .OnDelete(DeleteBehavior.Restrict);


            builder.Property(a => a.Notes)
                .HasMaxLength(350)
                .IsUnicode()
                .IsRequired();



          
        }
    }
}
