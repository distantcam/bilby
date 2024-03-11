using Bilby.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bilby;

public static class Helpers
{
    public static JsonHttpResult<TValue> ActivityStreamObject<TValue>(TValue? data)
        => Json(data, AppJsonSerializerContext.Default, contentType: $"{MimeTypes.ActivityJson}; charset=utf-8");
}
