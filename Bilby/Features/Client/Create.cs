using Bilby.Data;
using Bilby.Models;
using Bilby.Services;
using EndpointGenerator;

namespace Bilby.Features.Client;

public static class Create
{
    [EndpointGroupBuilder]
    public static void MapEndpoints(RouteGroupBuilder builder)
    {
        builder
            .MapPost("/api/post", HandleJson)
            .WithName("CreateJson")
            .RequireContentType(MimeTypes.Json)
            .RequireAuthorization();

        builder
            .MapPost("/api/post", HandleForm)
            .WithName("CreateForm")
            .RequireContentType(MimeTypes.Form, MimeTypes.FormData)
            .RequireAuthorization()
            .DisableAntiforgery();
    }

    public record struct CreateRequest(
        string Content,
        IEnumerable<string>? Media
    );

    private static Task<IResult> HandleJson(
        CreateRequest createRequest,
        AppDbContext appDbContext,
        UriGenerator uriGenerator,
        TimeProvider timeProvider
    ) => CreatePost(
        createRequest.Content,
        createRequest.Media,
        appDbContext,
        uriGenerator,
        timeProvider);

    private static async Task<IResult> HandleForm(
        IFormCollection form,
        IFormFileCollection formFiles,
        AppDbContext appDbContext,
        UriGenerator uriGenerator,
        TimeProvider timeProvider,
        StorageProvider storageProvider
    )
    {
        List<string> media = [];
        foreach (var file in formFiles)
        {
            var storageFile = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var uri = await storageProvider.UploadFile(
                "media",
                storageFile,
                file.ContentType,
                file.OpenReadStream()
            );
            media.Add(uri);
        }

        if (form.TryGetValue("media", out var photo))
            media.AddRange(photo);

        return await CreatePost(
            form["content"],
            media.Count != 0 ? media : null,
            appDbContext,
            uriGenerator,
            timeProvider);
    }

    private static async Task<IResult> CreatePost(
        string? content,
        IEnumerable<string>? media,
        AppDbContext appDbContext,
        UriGenerator uriGenerator,
        TimeProvider timeProvider
    )
    {
        if (string.IsNullOrEmpty(content))
            return BadRequest();

        var post = new Post
        {
            Content = content,
            PostedAt = timeProvider.GetUtcNow().DateTime,
            Media = media?.Select(m => new PostMedia(m)).ToList()
        };
        appDbContext.Posts.Add(post);
        await appDbContext.SaveChangesAsync();
        return Created(uriGenerator.GetPostUri(post.Id));
    }
}
