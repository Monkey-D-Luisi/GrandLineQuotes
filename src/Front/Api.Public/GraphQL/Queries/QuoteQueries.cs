using Api.Public.GraphQL.Models;
using AutoMapper;
using Client.Abstractions.Clients;
using GrandLineQuotes.Client.Abstractions.RequestModels.Quotes;

namespace Api.Public.GraphQL.Queries
{
    public class QuoteQueries
    {


        private readonly IApiClient _client;
        private readonly IMapper _mapper;


        public QuoteQueries(IApiClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }


        public async Task<IEnumerable<Quote>> Quotes(
            int? authorId,
            int? arcId,
            string? searchTerm) 
            => _mapper.Map<IEnumerable<Quote>>(
                await _client.ListQuotes(new QuotesListRequestModel
                {
                    AuthorId = authorId,
                    ArcId = arcId,
                    SearchTerm = searchTerm
                }));


        public async Task<Quote> Quote(int id) 
            => _mapper.Map<Quote>(
                await _client.GetQuote(id)
                );
    }
}
