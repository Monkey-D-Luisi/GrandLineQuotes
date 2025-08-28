using Application.Contexts.Episodes.Queries;
using Domain.Model.Episodes;
using Domain.Model.Episodes.Repositories;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using MediatR;

namespace Infrastructure.Contexts.Episodes.QueryHandlers
{
    internal class ListEpisodesQueryHandler : IRequestHandler<ListEpisodesQuery, IEnumerable<EpisodeDTO>>
    {


        private readonly IEpisodeRepository repository;


        public ListEpisodesQueryHandler(IEpisodeRepository repository)
        {
            this.repository = repository;
        }


        public async Task<IEnumerable<EpisodeDTO>> Handle(ListEpisodesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Episode> quotesList = await repository.List();

            return quotesList?.Select(quote => quote.GetSnapshot()) ?? new List<EpisodeDTO>();
        }
    }
}
