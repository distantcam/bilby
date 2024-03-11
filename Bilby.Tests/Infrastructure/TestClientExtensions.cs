using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Bilby.Tests.Infrastructure;

public static class TestClientExtensions
{
    public static IHttpClient UsingTestAuth(this IHttpClient outerClient)
    {
        return new ClientHelper(() =>
        {
            var client = outerClient.ClientFunc();
            client.DefaultRequestHeaders.Add("x-testing", "true");
            return client;
        });
    }

    public static IHttpClient WithASHeader(this IHttpClient outerClient)
    {
        return new ClientHelper(() =>
        {
            var client = outerClient.ClientFunc();
            client.DefaultRequestHeaders.Accept.Add(new("application/activity+json"));
            return client;
        });
    }

    public static IHttpClient WithBearerToken(this IHttpClient outerClient, string token)
    {
        return new ClientHelper(() =>
        {
            var client = outerClient.ClientFunc();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        });
    }

    private class ClientHelper(Func<HttpClient> clientFunc) : IHttpClient
    {
        public Func<HttpClient> ClientFunc => clientFunc;
    }

    public static Task<HttpResponseMessage> Get(this IHttpClient client, string uri) =>
        client.ClientFunc().GetAsync(uri);
    public static Task<HttpResponseMessage> Post(this IHttpClient client, string uri) =>
        client.ClientFunc().PostAsync(uri, null);
    public static Task<HttpResponseMessage> PostAsForm(this IHttpClient client, string uri, params (string Key, string Value)[] args) =>
        client.ClientFunc().PostAsync(uri, new FormUrlEncodedContent(args.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value))));
    public static Task<HttpResponseMessage> PostAsJson<TValue>(this IHttpClient client, string uri, TValue value) =>
        client.ClientFunc().PostAsJsonAsync(uri, value);
    public static Task<HttpResponseMessage> Delete(this IHttpClient client, string uri) =>
        client.ClientFunc().DeleteAsync(uri);
}
