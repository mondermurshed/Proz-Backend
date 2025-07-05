using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Feedbacks_AnswersConfiguration : IEntityTypeConfiguration<Feedbacks_Answers>
    {
        public void Configure(EntityTypeBuilder<Feedbacks_Answers> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(ed => ed.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(a => a.Answer)
                .HasMaxLength(1500)
                .IsUnicode()
                .IsRequired();

            builder.Property(a => a.ResponseDateTime)
                .HasColumnType("datetime2");

            builder.HasOne(a => a.EmployeeNA)
                .WithMany(e => e.FeedbackAnswerNA)
                .HasForeignKey(a => a.RespondentAccount_FK)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.FeedbackNA)
           .WithOne(f => f.FeedbacksAnswerNA)
           .HasForeignKey<Feedbacks_Answers>(a => a.Feedback_FK)
            .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

