namespace Bilby.Tests.Infrastructure;

public interface IHttpClient
{
    Func<HttpClient> ClientFunc { get; }
}
