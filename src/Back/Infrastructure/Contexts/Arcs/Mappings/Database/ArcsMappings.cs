using Domain.Model.Arcs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Contexts.Arcs.Mappings.Database
{
    internal class ArcsMappings : IEntityTypeConfiguration<Arc>
    {


        public void Configure(EntityTypeBuilder<Arc> builder)
        {
            builder.HasKey(p => p.Id);

            builder.ToTable("arc");

            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.FillerType)
                .HasConversion<int>()
                .HasColumnName("filler_type_id");

            builder.Property(p => p.SagaId).HasColumnName("saga_id");
            builder
                .HasOne(p => p.Saga)
                .WithMany()
                .HasForeignKey(p => p.SagaId);

            builder.OwnsMany(p => p.Titles, b =>
            {
                b.ToTable("arc_title");

                b.WithOwner().HasForeignKey(p => p.ParentId);

                b.Property(p => p.ParentId).HasColumnName("arc_id");
                b.Property(p => p.LanguageCode).HasColumnName("language_code");
                b.Property(p => p.Value).HasColumnName("value");

                b.HasKey(p => new { p.ParentId, p.LanguageCode });
            });
        }
    }
}
