using Application.Contexts.Arcs.Queries;
using Domain.Model.Arcs.Repositories;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;
using MediatR;

namespace Infrastructure.Contexts.Arcs.QueryHandlers
{
    internal class ListArcsQueryHandler : IRequestHandler<ListArcsQuery, IEnumerable<ArcDTO>>
    {


        private readonly IArcRepository arcRepository;


        public ListArcsQueryHandler(IArcRepository arcRepository)
        {
            this.arcRepository = arcRepository;
        }


        public async Task<IEnumerable<ArcDTO>> Handle(ListArcsQuery request, CancellationToken cancellationToken)
        {
            var arcs = await arcRepository.List(cancellationToken);

            return arcs.Select(arc => arc.GetSnapshot()) ?? new List<ArcDTO>();
        }
    }
}
