using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace Functions.Infrastructure.Extensions;

public static class CloudBlockBlobExtensions
{
    public static string GenerateSasUri(this BlockBlobClient blockBlobClient, UserDelegationKey userDelegationKey)
    {
        var blobServiceClient = blockBlobClient.GetParentBlobContainerClient().GetParentBlobServiceClient();

        var blobSasbuilder = new BlobSasBuilder
        {
            BlobContainerName = blockBlobClient.BlobContainerName,
            BlobName = blockBlobClient.Name,
            Resource = ResourceType.Blob,
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
        };
        blobSasbuilder.SetPermissions(BlobSasPermissions.Read);

        var blobUriBuilder = new BlobUriBuilder(blockBlobClient.Uri)
        {
            Sas = blobSasbuilder.ToSasQueryParameters(userDelegationKey, blobServiceClient.AccountName)
        };

        return blobUriBuilder.ToUri().ToString();
    }
}