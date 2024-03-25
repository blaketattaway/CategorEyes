using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Business.Analysis
{
    public interface IAnalysisBusiness
    {
        Task<AnalysisResponse> Analyze(AnalysisRequest request);
    }
}
