using System.Net.Http.Headers;

namespace Bilby.Startup;

public static class StartupExtensions
{
    public static IServiceCollection AddBilbyHttpClient(this IServiceCollection services)
    {
        return services.ConfigureHttpClientDefaults(config =>
        {
            var productValue = new ProductInfoHeaderValue("Bilby", null);
            config.ConfigureHttpClient(cc => cc.DefaultRequestHeaders.UserAgent.Add(productValue));
        });
    }
}
