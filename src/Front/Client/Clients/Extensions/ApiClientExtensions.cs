using Client.Abstractions.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Clients.Extensions
{
    public static class ApiClientExtensions
    {


        public static void AddApiClient(this IServiceCollection services)
        {
            services.AddTransient<IApiClient, ApiClient>();
        }
    }
}
