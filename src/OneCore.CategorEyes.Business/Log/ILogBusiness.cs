using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Business.Log
{
    public interface ILogBusiness
    {
        Task<LogResponse> GetPaged(LogRequest request);
    }
}
