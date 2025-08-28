using Google.Apis.Auth.OAuth2;
using Google.Apis.PlayIntegrity.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Google;
using GooglePlayIntegrityService = Google.Apis.PlayIntegrity.v1.PlayIntegrityService;

namespace Api.Public.Security
{
    public class PlayIntegrityService : IPlayIntegrityService
    {
        private readonly PlayIntegrityOptions _options;
        private readonly ILogger<PlayIntegrityService> _logger;
        private GooglePlayIntegrityService? _service;

        public PlayIntegrityService(IOptions<PlayIntegrityOptions> options, ILogger<PlayIntegrityService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<bool> ValidateAsync(string token, string nonce, string packageName)
        {
            try
            {
                _service ??= CreateService();
                var request = new DecodeIntegrityTokenRequest
                {
                    IntegrityToken = token
                };
                var response = await _service.V1
                    .DecodeIntegrityToken(request, packageName)
                    .ExecuteAsync();
                return response.TokenPayloadExternal?.RequestDetails?.Nonce == nonce;
            }
            catch (GoogleApiException ex)
            {
                _logger.LogWarning(ex, "Failed to decode Play Integrity token");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error validating Play Integrity token");
                return false;
            }
        }

        private GooglePlayIntegrityService CreateService()
        {
            var credential = GoogleCredential.FromFile(_options.ServiceAccountJsonPath)
                .CreateScoped(GooglePlayIntegrityService.ScopeConstants.Playintegrity);

            return new GooglePlayIntegrityService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });
        }
    }
}
