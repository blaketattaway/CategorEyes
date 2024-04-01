using OneCore.CategorEyes.Commons.Blob;

namespace OneCore.CategorEyes.Business.Services
{
    public interface IBlobService
    {
        Task<string> UploadFile(FileUpload fileUpload);
    }
}
