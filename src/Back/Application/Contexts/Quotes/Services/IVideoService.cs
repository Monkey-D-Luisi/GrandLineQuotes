namespace Application.Contexts.Quotes.Services
{
    public interface IVideoService
    {


        public Task<bool> Exists(string fileName, CancellationToken cancellationToken);
        public Task Upload(Stream stream, string fileName, string contentType, CancellationToken cancellationToken);
        public Task Delete(string fileName, CancellationToken cancellationToken);
    }
}
