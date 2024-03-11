using System.Text.RegularExpressions;
using Bilby.Data;
using Bilby.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using static Bilby.Features.Client.Create;

public class ClientTests(IntegrationTestWebAppFactory factory) : BaseEndpointTests(factory)
{
    [Fact]
    public async Task CreateJson()
    {
        await Verify(await this.UsingTestAuth().PostAsJson("/api/post",
            new CreateRequest($"<p>{nameof(CreateJson)} Test</p>", null)))
            .ScrubLinesWithReplace(s => Regex.Replace(s, "localhost/post/\\d+", "localhost/post/SCRUBBED_ID"));
    }

    [Fact]
    public async Task CreateForm()
    {
        await Verify(await this.UsingTestAuth().PostAsForm(
            "/api/post",
            ("content", $"<p>{nameof(CreateForm)} Test</p>")))
            .ScrubLinesWithReplace(s => Regex.Replace(s, "localhost/post/\\d+", "localhost/post/SCRUBBED_ID"));
    }

    [Fact]
    public async Task CreateForm_MissingContent()
    {
        await Verify(await this.UsingTestAuth().PostAsForm("/api/post"));
    }

    [Fact]
    public async Task Delete()
    {
        // Arrange
        var post = new Post { Content = "<p>{nameof(Delete)} Test</p>", PostedAt = DateTime.UtcNow };
        ArrangeDbContext.Posts.Add(post);
        await ArrangeDbContext.SaveChangesAsync();

        // Act
        var response = await this.UsingTestAuth().Delete($"/api/post/{post.Id}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        post = await AssertDbContext.Posts.FirstAsync(p => p.Id == post.Id);
        post.IsDeleted.Should().BeTrue();
    }
}
