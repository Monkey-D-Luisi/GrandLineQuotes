using Domain.Model.Episodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Contexts.Episodes.Mappings.Database
{
    internal class EpisodesMappings : IEntityTypeConfiguration<Episode>
    {


        public void Configure(EntityTypeBuilder<Episode> builder)
        {
            builder.HasKey(p => p.Number);

            builder.ToTable("episode");

            builder.Property(p => p.Number).HasColumnName("number");

            builder.Property(p => p.ArcId).HasColumnName("arc_id");
            builder
                .HasOne(p => p.Arc)
                .WithMany()
                .HasForeignKey(p => p.ArcId);

            builder.OwnsMany(p => p.Titles, b =>
            {
                b.ToTable("episode_title");

                b.WithOwner().HasForeignKey(p => p.ParentId);

                b.Property(p => p.ParentId).HasColumnName("episode_number");
                b.Property(p => p.LanguageCode).HasColumnName("language_code");
                b.Property(p => p.Value).HasColumnName("value");

                b.HasKey(p => new { p.ParentId, p.LanguageCode });
            });
        }
    }
}
