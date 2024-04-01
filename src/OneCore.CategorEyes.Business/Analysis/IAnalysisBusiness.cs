using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;

namespace OneCore.CategorEyes.Business.Analysis
{
    public interface IAnalysisBusiness
    {
        Task<AnalysisResponse> Analyze(AnalysisRequest request);
    }
}
