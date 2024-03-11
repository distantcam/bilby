using System.Text.Json;
using AutoCtor;
using Bilby.ActivityStream;
using Bilby.Data;
using Injectio.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Services;

[AutoConstruct]
[RegisterScoped]
public partial class AccountLookupManager
{
    private readonly ILogger<AccountLookupManager> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public async Task<string?> GetAccountPublicKey(string actorId)
    {
        var publicKey = await _appDbContext.Accounts
            .Where(a => a.ActorUrl == actorId)
            .Select(a => a.PublicKeyPem)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (publicKey == null)
        {
            var account = await CreateAccountFromActor(actorId).ConfigureAwait(false);
            publicKey = account?.PublicKeyPem;
        }
        return publicKey;
    }

    public async Task<Account?> GetAccount(string actorId)
    {
        return await _appDbContext.Accounts
            .FirstOrDefaultAsync(a => a.ActorUrl == actorId)
            .ConfigureAwait(false)
        ?? await CreateAccountFromActor(actorId).ConfigureAwait(false);
    }

    private async Task<Account?> CreateAccountFromActor(string actorId)
    {
        var actor = await FetchActor(actorId);

        if (actor is null)
        {
            _logger.LogWarning("Failed trying to get actors key");
            return null;
        }

        if (actor["publicKey"] is null)
        {
            _logger.LogWarning("Actor did not contain a public key");
            return null;
        }

        var publicKey = actor["publicKey"]["publicKeyPem"].AsString();

        if (string.IsNullOrEmpty(publicKey))
            return null;

        // Store actor in account
        var account = new Account
        {
            ActorUrl = actor.Id.AsString()!,
            InboxUrl = actor.Inbox?.AsString(),
            OutboxUrl = actor.Outbox?.AsString(),
            FollowersUrl = actor.Followers?.AsString(),
            FollowingUrl = actor.Following?.AsString(),
            PublicKeyPem = publicKey
        };
        _appDbContext.Add(account);
        await _appDbContext.SaveChangesAsync().ConfigureAwait(false);

        return account;
    }

    private async Task<Person?> FetchActor(string actorId)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/activity+json");
        var result = await httpClient.GetAsync(actorId).ConfigureAwait(false);

        result.EnsureSuccessStatusCode();
        var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize(content, AppJsonSerializerContext.Default.Person);
    }
}
