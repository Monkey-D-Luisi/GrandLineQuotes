using Domain.Model.Sagas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Contexts.Sagas.Mappings.Database
{
    internal class SagasMappings : IEntityTypeConfiguration<Saga>
    {


        public void Configure(EntityTypeBuilder<Saga> builder)
        {
            builder.HasKey(p => p.Id);

            builder.ToTable("saga");

            builder.Property(p => p.Id).HasColumnName("id");

            builder.OwnsMany(p => p.Titles, b =>
            {
                b.ToTable("saga_title");

                b.WithOwner().HasForeignKey(p => p.ParentId);

                b.Property(p => p.ParentId).HasColumnName("saga_id");
                b.Property(p => p.LanguageCode).HasColumnName("language_code");
                b.Property(p => p.Value).HasColumnName("value");

                b.HasKey(p => new { p.ParentId, p.LanguageCode });
            });
        }
    }
}
