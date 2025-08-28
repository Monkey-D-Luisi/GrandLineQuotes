namespace Api.Public.Security
{
    public interface IPlayIntegrityService
    {
        Task<bool> ValidateAsync(string token, string nonce, string packageName);
    }
}
