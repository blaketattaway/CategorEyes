using OneCore.CategorEyes.Commons.Blob;

namespace OneCore.CategorEyes.Business.Services
{
    public interface IBlobService
    {
        /// <summary>
        /// Asynchronously uploads a file to Azure Blob Storage. The file content is provided as a base64-encoded string. A unique file name is generated for the upload.
        /// </summary>
        /// <param name="fileUpload">An instance of <see cref="FileUpload"/> containing the base64-encoded file content, the content type, and the file extension, of type <see cref="FileUpload"/>.</param>
        /// <returns>A task representing the asynchronous operation, returning the unique file name of the uploaded blob as a <see cref="string"/>.</returns>
        Task<string> UploadFile(FileUpload fileUpload, bool isReport = false);
    }
}
