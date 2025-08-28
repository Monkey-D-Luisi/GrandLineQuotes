using Api.Public.GraphQL.Models;

namespace Api.Public.GraphQL.Bindings
{
    public class QuoteType : ObjectType<Quote>
    {
        

        protected override void Configure(IObjectTypeDescriptor<Quote> descriptor)
        {
            descriptor.Ignore(quote => quote.Video);
        }
    }
}
