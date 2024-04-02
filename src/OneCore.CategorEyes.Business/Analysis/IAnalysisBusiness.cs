using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;

namespace OneCore.CategorEyes.Business.Analysis
{
    public interface IAnalysisBusiness
    {
        /// <summary>
        /// Analiza el documento proporcionado en la solicitud utilizando el servicio de OpenAI y registra los resultados.
        /// </summary>
        /// <param name="request">La solicitud de análisis, de tipo <see cref="AnalysisRequest"/>.</param>
        /// <returns>Una tarea que representa la operación asincrónica y retorna una <see cref="AnalysisResponse"/> con los resultados del análisis.</returns>
        Task<AnalysisResponse> Analyze(AnalysisRequest request);
    }
}
