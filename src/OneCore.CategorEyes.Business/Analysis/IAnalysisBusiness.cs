using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;

namespace OneCore.CategorEyes.Business.Analysis
{
    public interface IAnalysisBusiness
    {
        /// <summary>
        /// Analyzes the document provided in the request using the OpenAI service and logs the results.
        /// </summary>
        /// <param name="request">The analysis request, of type <see cref="AnalysisRequest"/>.</param>
        /// <returns>A task representing the asynchronous operation, returning an <see cref="AnalysisResponse"/> with the analysis results.</returns>
        Task<AnalysisResponse> Analyze(AnalysisRequest request);
    }
}
