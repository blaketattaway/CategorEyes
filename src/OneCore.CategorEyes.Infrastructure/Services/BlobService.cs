using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OneCore.CategorEyes.Business.Services;
using OneCore.CategorEyes.Commons.Blob;
using OneCore.CategorEyes.Commons.Consts;

namespace OneCore.CategorEyes.Infrastructure.Services
{
    internal class BlobService : IBlobService
    {
        private readonly BlobContainerClient _blobContainerClient;
        private readonly ILogger<BlobService> _logger;

        private static readonly string BlobStorageKeyName = Blob.BLOB_STORAGE_KEY_NAME;
        private static readonly string ContainerName = Blob.CONTAINER_NAME;

        /// <summary>
        /// Initializes a new instance of the BlobService class using the provided application configuration and logger.
        /// </summary>
        /// <param name="configuration">The application configuration containing blob storage settings, provided via <see cref="IConfiguration"/>.</param>
        /// <param name="logger">The logger for capturing runtime information and errors, provided via <see cref="ILogger{BlobService}"/>.</param>
        public BlobService(IConfiguration configuration, ILogger<BlobService> logger)
        {
            _blobContainerClient = InitializeBlobContainerClient(configuration);
            _logger = logger;
        }

        public async Task<string> UploadFile(FileUpload fileUpload)
        {
            try
            {
                string fileName = GenerateFileName(fileUpload.Extension);
                await using var fileToUploadStream = ConvertBase64ToFileStream(fileUpload.Base64File);

                var blobClient = _blobContainerClient.GetBlobClient(fileName);
                var options = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = fileUpload.ContentType }
                };

                await blobClient.UploadAsync(fileToUploadStream, options);
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to Blob Storage.");
                throw;
            }
        }

        /// <summary>
        /// Initializes and returns a BlobContainerClient using the connection string and container name provided in the application configuration.
        /// </summary>
        /// <param name="configuration">The application configuration from which to retrieve the blob storage connection string and container name, of type <see cref="IConfiguration"/>.</param>
        /// <returns>A <see cref="BlobContainerClient"/> for interacting with the specified Azure Blob Storage container.</returns>
        private BlobContainerClient InitializeBlobContainerClient(IConfiguration configuration)
        {
            var connectionString = configuration[BlobStorageKeyName];
            var containerName = configuration[ContainerName];
            return new BlobContainerClient(connectionString, containerName);
        }

        /// <summary>
        /// Generates a unique file name using a GUID, appended with the provided file extension.
        /// </summary>
        /// <param name="fileExtension">The file extension to append to the generated file name, of type <see cref="string"/>.</param>
        /// <returns>A unique file name as a <see cref="string"/>.</returns>
        private static string GenerateFileName(string fileExtension)
        {
            return $"{Guid.NewGuid()}.{fileExtension}";
        }

        /// <summary>
        /// Converts a base64-encoded string to a MemoryStream, ready for upload or processing.
        /// </summary>
        /// <param name="base64String">The base64-encoded string representing the file content, of type <see cref="string"/>.</param>
        /// <returns>A <see cref="MemoryStream"/> containing the decoded file content.</returns>
        private static MemoryStream ConvertBase64ToFileStream(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return stream;
        }
    }
}
