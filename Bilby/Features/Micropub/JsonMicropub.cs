using System.Text.Json;
using Bilby.Data;
using Bilby.Models;
using Bilby.Services;
using EndpointGenerator;
using Microsoft.EntityFrameworkCore;
using static Bilby.Models.MicroFormats2Object;

namespace Bilby.Features.Micropub;

public static class JsonMicropub
{
    [EndpointGroupBuilder]
    public static void JsonPost(RouteGroupBuilder builder) => builder
        .MapPost("/micropub", HandleJson)
        .RequireContentType(MimeTypes.Json)
        .RequireAuthorization();

    public record struct HtmlContent(string Html);
    public record struct PhotoWithAlt(string Value, string Alt);

    private static async Task<IResult> HandleJson(
        MicroFormats2Object micropubRequest,
        AppDbContext appDbContext,
        UriGenerator uriGenerator,
        TimeProvider timeProvider
    )
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(micropubRequest.Type?.FirstOrDefault(), "h-entry"))
        {
            return await CreateEntry(micropubRequest, appDbContext, uriGenerator, timeProvider);
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(micropubRequest.Action, "update"))
        {
            return await UpdateEntry(micropubRequest, appDbContext, uriGenerator);
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(micropubRequest.Action, "delete") ||
            StringComparer.OrdinalIgnoreCase.Equals(micropubRequest.Action, "undelete"))
        {
            return await DeleteEntry(micropubRequest, appDbContext, uriGenerator);
        }
        return BadRequest();
    }

    private static async Task<IResult> CreateEntry(
        MicroFormats2Object micropubRequest,
        AppDbContext appDbContext,
        UriGenerator uriGenerator,
        TimeProvider timeProvider)
    {
        var jsonContent = micropubRequest.Properties["content"].First();
        var content = $"<p>{jsonContent}</p>";
        if (jsonContent.IsObject)
        {
            var htmlContent = JsonSerializer.Deserialize(jsonContent, AppJsonSerializerContext.Default.HtmlContent);
            content = htmlContent.Html;
        }

        var photos = micropubRequest.Properties.GetValueOrDefault("photo")?
            .Select(CreatePhoto)?
            .ToList();

        string[] knownProperties = ["content", "category", "photo"];
        //var extras = micropubRequest.Properties
        //    .Where(kv => !knownProperties.Contains(kv.Key))
        //    .Where(kv => kv.Value.First().IsObject)
        //    .Select(kv => new EntryObject { Key = kv.Key, Value = kv.Value.First() })
        //    .ToList();

        var post = new Post
        {
            Content = content,
            //Categories = micropubRequest.Properties.GetValueOrDefault("category")?.Select(x => (string)x)?.ToList(),
            Media = photos,
            PostedAt = timeProvider.GetUtcNow().DateTime,
            //Objects = extras
        };
        //appDbContext.Entries.Add(entry);
        await appDbContext.SaveChangesAsync();

        return Created(uriGenerator.GetPostUri(post.Id));
    }

    private static async Task<IResult> UpdateEntry(
        MicroFormats2Object micropubRequest,
        AppDbContext appDbContext,
        UriGenerator uriGenerator)
    {
        var postId = uriGenerator.GetPostId(micropubRequest.Url);
        var post = await appDbContext.Posts
            .FirstAsync(p => p.Id == postId);

        if (micropubRequest.Replace is not null)
        {
            if (micropubRequest.Replace.TryGetValue("content", out var content))
            {
                post.Content = $"<p>{content.First()}</p>";
            }
        }

        //if (micropubRequest.Add is not null)
        //{
        //    if (micropubRequest.Add.TryGetValue("category", out var categories))
        //    {
        //        if (post.Categories is null)
        //            post.Categories = [];
        //        post.Categories!.AddRange(categories.Select(x => (string)x));
        //    }
        //}

        //if (micropubRequest.Delete is not null)
        //{
        //    if (post.Categories is not null && micropubRequest.Delete.TryGetValue("category", out var categories))
        //    {
        //        if (categories.Any())
        //            foreach (var category in categories)
        //            {
        //                post.Categories.Remove(category);
        //            }
        //        else
        //        {
        //            // Remove all
        //            post.Categories = null;
        //        }
        //    }
        //}

        await appDbContext.SaveChangesAsync();
        return NoContent();
    }

    private static async Task<IResult> DeleteEntry(
        MicroFormats2Object micropubRequest,
        AppDbContext appDbContext,
        UriGenerator uriGenerator)
    {
        var postId = uriGenerator.GetPostId(micropubRequest.Url);
        var post = await appDbContext.Posts
            .FirstAsync(e => e.Id == postId);

        post.IsDeleted = StringComparer.OrdinalIgnoreCase.Equals(micropubRequest.Action, "delete");
        await appDbContext.SaveChangesAsync();

        return NoContent();
    }

    private static PostMedia CreatePhoto(JsonString jsonString)
    {
        if (!jsonString.IsObject)
            return new(jsonString);

        var json = JsonSerializer.Deserialize(jsonString, AppJsonSerializerContext.Default.PhotoWithAlt);
        return new(json.Value, json.Alt);
    }
}
