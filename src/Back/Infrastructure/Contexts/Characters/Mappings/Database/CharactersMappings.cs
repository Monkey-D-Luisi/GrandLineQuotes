using Domain.Model.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Contexts.Characters.Mappings.Database
{
    internal class CharactersMappings : IEntityTypeConfiguration<Character>
    {


        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.HasKey(p => p.Id);

            builder.ToTable("character");

            builder
                .Property(p => p.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd()
                .UseMySqlIdentityColumn();
            builder.Property(p => p.Name).HasColumnName("name");
            builder.Property(p => p.Alias).HasColumnName("alias");
        }
    }
}
