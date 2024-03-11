using Bilby.Data;
using Bilby.Models;
using Bilby.Services;
using EndpointGenerator;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Features.Micropub;

public static class FormMicropub
{
    [EndpointGroupBuilder]
    public static void FormPost(RouteGroupBuilder builder) => builder
        .MapPost("/micropub", HandleForm)
        .RequireContentType(MimeTypes.Form, MimeTypes.FormData)
        .RequireAuthorization()
        .DisableAntiforgery();

    private static async Task<IResult> HandleForm(
        IFormCollection form,
        IFormFileCollection formFiles,
        AppDbContext appDbContext,
        UriGenerator uriGenerator,
        TimeProvider timeProvider,
        StorageProvider storageProvider
    )
    {
        if (form.TryGetValue("h", out var h) && StringComparer.OrdinalIgnoreCase.Equals(h, "entry"))
        {
            return await CreateEntry(form, formFiles, appDbContext, uriGenerator, timeProvider, storageProvider);
        }
        if (form.TryGetValue("action", out var action))
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(action, "delete") ||
                StringComparer.OrdinalIgnoreCase.Equals(action, "undelete"))
            {
                return await DeleteEntry(form, appDbContext, uriGenerator);
            }
        }
        return BadRequest();
    }

    private static async Task<IResult> CreateEntry(
        IFormCollection form,
        IFormFileCollection formFiles,
        AppDbContext appDbContext,
        UriGenerator uriGenerator,
        TimeProvider timeProvider,
        StorageProvider storageProvider)
    {
        var content = form["content"];

        if (string.IsNullOrEmpty(content))
            return BadRequest();

        List<PostMedia> postMedia = [];
        foreach (var file in formFiles)
        {
            var storageFile = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var uri = await storageProvider.UploadFile(
                "images",
                storageFile,
                file.ContentType,
                file.OpenReadStream()
            );
            postMedia.Add(new(uri));
        }

        //List<string>? entityCategories = form.TryGetValue("category", out var category)
        //    ? [category!]
        //    : form.TryGetValue("category[]", out var categories) && categories.Count != 0
        //    ? [.. categories]
        //    : null;

        if (form.TryGetValue("photo", out var photo) && !string.IsNullOrEmpty(photo))
            postMedia.Add(new(photo!));

        var post = new Post
        {
            Content = $"<p>{content}</p>",
            //Categories = entityCategories,
            Media = postMedia,
            PostedAt = timeProvider.GetUtcNow().DateTime
        };
        appDbContext.Posts.Add(post);
        appDbContext.SaveChanges();

        return Created(uriGenerator.GetPostUri(post.Id));
    }

    private static async Task<IResult> DeleteEntry(
        IFormCollection form,
        AppDbContext appDbContext,
        UriGenerator uriGenerator)
    {
        if (!form.TryGetValue("url", out var url) || string.IsNullOrEmpty(url))
            return BadRequest();

        var action = form["action"];

        var postId = uriGenerator.GetPostId(url!);
        var post = await appDbContext.Posts
            .FirstAsync(e => e.Id == postId);
        post.IsDeleted = StringComparer.OrdinalIgnoreCase.Equals(action, "delete");
        await appDbContext.SaveChangesAsync();

        return NoContent();
    }
}
