using OneCore.CategorEyes.Commons.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Business.Services
{
    public interface IBlobService
    {
        Task<string> UploadFile(FileUpload fileUpload);
    }
}
