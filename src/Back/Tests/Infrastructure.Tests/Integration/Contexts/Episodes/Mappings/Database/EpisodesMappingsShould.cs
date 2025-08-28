using Domain.Model.Episodes;
using FluentAssertions;
using Tests.Core;

namespace Infrastructure.Tests.Integration.Contexts.Episodes.Mappings.Database
{
    [TestFixture(Category = "Integration")]
    internal class EpisodesMappingsShould : IntegrationTestBase
    {


        [Test]
        public void Map_the_entity_from_database()
        {
            var entityType = dbContext.Model.FindEntityType(typeof(Episode));
            entityType.Should().NotBeNull();
            entityType?.FindPrimaryKey().Should().NotBeNull();
        }
    }
}
