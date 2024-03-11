using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Data;

[PrimaryKey(nameof(Id))]
public record StoredSetting
{
    public long Id { get; set; }
    public required string Key { get; init; }
    public required string Value { get; set; }

    public StoredSetting()
    {
    }

    [SetsRequiredMembers]
    public StoredSetting(string key, string value)
    {
        Key = key;
        Value = value;
    }
}
