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
        /// <exception cref="ArgumentException">Thrown when the analysis request contains invalid or unsupported values.</exception>
        /// <exception cref="Exception">Thrown if there's an error uploading the file to the blob storage, or if any unexpected errors occur during the analysis process.</exception>
        Task<AnalysisResponse> Analyze(AnalysisRequest request);
    }
}
