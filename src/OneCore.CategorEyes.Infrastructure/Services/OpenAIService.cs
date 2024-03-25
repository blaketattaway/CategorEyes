using OneCore.CategorEyes.Business.Services;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using OneCore.CategorEyes.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Infrastructure.Services
{
    internal class OpenAIService : IOpenAIService
    {
        public async Task<OpenAIAnalysisResponse> Analyze(object request)
        {
            return await new RestConsumer(BaseAPI.OpenAIAPI).PostResponse<OpenAIAnalysisResponse, object>("chat/completions", request);
        }
    }
}
