using Domain.Model.Quotes;
using FluentAssertions;
using Tests.Core;

namespace Infrastructure.Tests.Integration.Contexts.Quotes.Mappings.Database
{
    [TestFixture(Category = "Integration")]
    public class QuotesMappingsShould : IntegrationTestBase
    {


        [Test]
        public void Map_the_entity_from_database() 
        {
            var entityType = dbContext.Model.FindEntityType(typeof(Quote));
            entityType.Should().NotBeNull();
            entityType?.FindPrimaryKey().Should().NotBeNull();
        }
    }
}
