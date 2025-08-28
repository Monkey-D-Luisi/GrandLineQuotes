using MediatR;

namespace Application.Contexts.Episodes.Commands
{
    public record DeleteEpisodeCommand(int Number) : IRequest;
}
