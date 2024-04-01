using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;

namespace OneCore.CategorEyes.Business.Log
{
    public interface ILogBusiness
    {
        Task<LogResponse> GetPaged(LogRequest request);
    }
}
