using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Feedback_TypesConfiguration : IEntityTypeConfiguration<Feedback_Types>
    {
        public void Configure(EntityTypeBuilder<Feedback_Types> builder)
        {
            builder.HasKey(ft => ft.id);
            builder.Property(ed => ed.id).HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(ft => ft.FeedbackType)
                .HasMaxLength(15)
                .IsUnicode()
                .IsRequired();

            builder.HasMany(ft => ft.FeedbacksNA)
            .WithOne(f => f.FeedbackTypeNA)
            .HasForeignKey(f => f.FeedbackType_FK);
        }
        }
}
