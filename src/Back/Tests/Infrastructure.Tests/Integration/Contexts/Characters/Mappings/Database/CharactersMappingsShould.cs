using Domain.Model.Characters;
using FluentAssertions;
using Tests.Core;

namespace Infrastructure.Tests.Integration.Contexts.Characters.Mappings.Database
{
    [TestFixture(Category = "Integration")]
    public class CharactersMappingsShould : IntegrationTestBase
    {


        [Test]
        public void Map_the_entity_from_database() 
        {
            var entityType = dbContext.Model.FindEntityType(typeof(Character));
            entityType.Should().NotBeNull();
            entityType?.FindPrimaryKey().Should().NotBeNull();
        }
    }
}
