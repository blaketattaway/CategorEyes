using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using OneCore.CategorEyes.Business.Services;
using OneCore.CategorEyes.Commons.Blob;
using OneCore.CategorEyes.Commons.Consts;

namespace OneCore.CategorEyes.Infrastructure.Services
{
    internal class BlobService : IBlobService
    {
        private readonly BlobContainerClient _blobContainerClient;

        public BlobService(IConfiguration configuration)
        {
            _blobContainerClient = new BlobContainerClient(configuration[Blob.BLOB_STORAGE_KEY_NAME], configuration[Blob.CONTAINER_NAME]);
        }

        public async Task<string> UploadFile(FileUpload fileUpload)
        {
            string fileExtension = fileUpload.Extension;
            string fileName = $"{Guid.NewGuid()}.{fileExtension}";
            byte[] bytes = Convert.FromBase64String(fileUpload.Base64File);
            using MemoryStream fileToUploadStream = new();
            fileToUploadStream.Write(bytes, 0, bytes.Length);
            fileToUploadStream.Position = 0;
            var blobClient = _blobContainerClient.GetBlobClient(fileName);
            BlobUploadOptions options = new()
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = fileUpload.ContentType },
            };
            await blobClient.UploadAsync(fileToUploadStream, options);
            return fileName;
        }
    }
}
