using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class FeedbacksConfigurationcs : IEntityTypeConfiguration<Feedbacks>
    {
        public void Configure(EntityTypeBuilder<Feedbacks> builder)
        {
            builder.HasKey(f => f.id);
            builder.Property(ed => ed.id).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(f => f.FeedbackTitle)
                .HasMaxLength(80)
                .IsUnicode()
                .IsRequired();

            builder.Property(f => f.FeedbackDescription)
                .HasMaxLength(1500)
                .IsUnicode()
                .IsRequired();

            builder.Property(f => f.LastUpdated)
                .HasColumnType("datetime2");

            builder.HasOne(f => f.EmployeeNA)
                .WithMany(e => e.FeedbacksNA)
                .HasForeignKey(f => f.Employee_FK)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.FeedbacksAnswerNA)
                .WithOne(a => a.FeedbackNA)
                .HasForeignKey<Feedbacks_Answers>(a => a.Feedback_FK);

            builder.HasOne(f => f.FeedbackTypeNA)
            .WithMany(ft => ft.FeedbacksNA)
            .HasForeignKey(f => f.FeedbackType_FK);
               
        }
    }
}
