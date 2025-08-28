using Domain.Model.Quotes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Contexts.Quotes.Mappings.Database
{
    internal class QuotesMappings : IEntityTypeConfiguration<Quote>
    {


        public void Configure(EntityTypeBuilder<Quote> builder)
        {
            builder.HasKey(p => p.Id);

            builder.ToTable("quote");

            builder
                .Property(p => p.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd()
                .UseMySqlIdentityColumn();
            builder.Property(p => p.OriginalText).HasColumnName("original_text");
            builder.Property(p => p.Text).HasColumnName("text");
            builder.Property(p => p.AuthorId).HasColumnName("author_id");
            builder.Property(p => p.EpisodeNumber).HasColumnName("episode_number");
            builder.Property(p => p.IsReviewed).HasColumnName("is_reviewed");

            builder
                .HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId);

            builder
                .HasOne(p => p.Episode)
                .WithMany()
                .HasForeignKey(p => p.EpisodeNumber);

            builder.OwnsMany(p => p.Translations, b =>
            {
                b.ToTable("quote_translation");

                b.WithOwner().HasForeignKey(p => p.ParentId);

                b.Property(p => p.ParentId).HasColumnName("quote_id");
                b.Property(p => p.LanguageCode).HasColumnName("language_code");
                b.Property(p => p.Value).HasColumnName("value");

                b.HasKey(p => new { p.ParentId, p.LanguageCode });
            });
        }
    }
}
