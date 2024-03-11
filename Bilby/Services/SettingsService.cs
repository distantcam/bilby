using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bilby.Data;
using Injectio.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Services;

[RegisterSingleton]
public class SettingsService(IDbContextFactory<AppDbContext> contextFactory) : IDictionary<string, string>
{
    public static readonly string PasswordKey = "Password";
    public static readonly string ServerTitleKey = "ServerTitle";
    public static readonly string ShortDescriptionKey = "ShortDescription";
    public static readonly string DescriptionKey = "Description";
    public static readonly string EmailKey = "Email";

    public static readonly string UsernameKey = "Username";
    public static readonly string DisplayNameKey = "DisplayName";
    public static readonly string SummaryKey = "Summary";

    public string this[string key]
    {
        get
        {
            using var appDbContext = contextFactory.CreateDbContext();
            var setting = appDbContext.StoredSettings.AsNoTracking().FirstOrDefault(s => s.Key == key);
            return setting is null
                ? throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.")
                : setting.Value;
        }
        set
        {
            using var appDbContext = contextFactory.CreateDbContext();
            var setting = appDbContext.StoredSettings.FirstOrDefault(s => s.Key == key);
            if (setting is null)
            {
                setting = new StoredSetting(key, value);
                appDbContext.StoredSettings.Add(setting);
            }
            else
            {
                setting.Value = value;
            }
            appDbContext.SaveChanges();
        }
    }

    public ICollection<string> Keys => throw new NotImplementedException();
    public ICollection<string> Values => throw new NotImplementedException();

    public int Count
    {
        get
        {
            using var appDbContext = contextFactory.CreateDbContext();
            return appDbContext.StoredSettings.Count();
        }
    }
    public bool IsReadOnly => false;

    public void Add(string key, string value)
    {
        using var appDbContext = contextFactory.CreateDbContext();
        if (appDbContext.StoredSettings.Any(x => x.Key == key))
            throw new ArgumentException($"An item with the same key has already been added. Key: {key}");
        appDbContext.StoredSettings.Add(new(key, value));
        appDbContext.SaveChanges();
    }
    public bool Remove(string key)
    {
        using var appDbContext = contextFactory.CreateDbContext();
        var setting = appDbContext.StoredSettings.FirstOrDefault(s => s.Key == key);
        if (setting is null)
            return false;
        appDbContext.StoredSettings.Remove(setting);
        appDbContext.SaveChanges();
        return true;
    }
    public void Clear()
    {
        using var appDbContext = contextFactory.CreateDbContext();
        appDbContext.StoredSettings.RemoveRange(appDbContext.StoredSettings);
        appDbContext.SaveChanges();
    }

    public bool ContainsKey(string key)
    {
        using var appDbContext = contextFactory.CreateDbContext();
        return appDbContext.StoredSettings.Any(x => x.Key == key);
    }
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
    {
        using var appDbContext = contextFactory.CreateDbContext();
        var setting = appDbContext.StoredSettings.AsNoTracking().FirstOrDefault(s => s.Key == key);
        if (setting is null)
        {
            value = null;
            return false;
        }
        value = setting.Value;
        return true;
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        using var appDbContext = contextFactory.CreateDbContext();
        return appDbContext.StoredSettings
            .Select(s => new KeyValuePair<string, string>(s.Key, s.Value))
            .ToImmutableList()
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item) =>
        Add(item.Key, item.Value);
    bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item) =>
        Remove(item.Key);
    bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item) =>
        ContainsKey(item.Key);
    void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => throw new NotImplementedException();
}
