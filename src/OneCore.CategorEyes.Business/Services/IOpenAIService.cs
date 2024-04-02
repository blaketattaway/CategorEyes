using OneCore.CategorEyes.Commons.Responses;

namespace OneCore.CategorEyes.Business.Services
{
    public interface IOpenAIService
    {
        /// <summary>
        /// Asynchronously sends a request to the OpenAI API to analyze the provided data and returns the analysis result.
        /// </summary>
        /// <param name="request">The request object containing the data to be analyzed by OpenAI, of type <see cref="object"/>. The actual type of the object should match the expected structure of the OpenAI API request.</param>
        /// <returns>A task representing the asynchronous operation, returning the analysis result from OpenAI, of type <see cref="Task{OpenAIAnalysisResponse}"/>.</returns>
        Task<OpenAIAnalysisResponse> Analyze(object request);
    }
}
