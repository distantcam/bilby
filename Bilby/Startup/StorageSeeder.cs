using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Startup;

public class StorageSeeder(IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("BlobStorage"));
        await blobServiceClient.GetBlobContainerClient("media")
            .CreateIfNotExistsAsync(PublicAccessType.BlobContainer, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
