using Domain.Model.Arcs;
using Domain.Model.Characters;
using Domain.Model.Episodes;
using Domain.Model.Quotes;
using Domain.Model.Sagas;
using Infrastructure.Contexts.Arcs.Mappings.Database;
using Infrastructure.Contexts.Characters.Mappings.Database;
using Infrastructure.Contexts.Episodes.Mappings.Database;
using Infrastructure.Contexts.Quotes.Mappings.Database;
using Infrastructure.Contexts.Sagas.Mappings.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.DatabaseContexts
{
    public class ApplicationDbContext : DbContext
    {


        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Arc> Arcs { get; set; }
        public DbSet<Saga> Sagas { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new QuotesMappings());
            modelBuilder.ApplyConfiguration(new CharactersMappings());
            modelBuilder.ApplyConfiguration(new EpisodesMappings());
            modelBuilder.ApplyConfiguration(new ArcsMappings());
            modelBuilder.ApplyConfiguration(new SagasMappings());
        }
    }
}
