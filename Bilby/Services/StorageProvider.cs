using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Injectio.Attributes;

namespace Bilby.Services;

[RegisterScoped]
public class StorageProvider(IConfiguration configuration)
{
    private readonly BlobServiceClient _blobServiceClient = new(configuration.GetConnectionString("BlobStorage"));

    public async Task<string> UploadFile(
        string containerName,
        string filename,
        string contentType,
        Stream fileStream,
        CancellationToken cancellationToken = default)
    {
        fileStream.Seek(0, SeekOrigin.Begin);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(filename);

        await blobClient.UploadAsync(
            fileStream,
            new BlobHttpHeaders
            {
                ContentType = contentType
            },
            cancellationToken: cancellationToken,
            conditions: null);

        return blobClient.Uri.AbsoluteUri;
    }
}
