using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;

namespace OneCore.CategorEyes.Business.Report
{
    public interface IReportBusiness
    {
        /// <summary>
        /// Generates a report based on the provided <see cref="ReportRequest"/> parameters, stores it in a blob storage, and returns the report's URL.
        /// </summary>
        /// <param name="request">The <see cref="ReportRequest"/> containing parameters for the report generation.</param>
        /// <returns>A <see cref="Task{ReportResponse}"/> representing the asynchronous operation, containing the URL to the generated report.</returns>
        Task<ReportResponse> GenerateReport(ReportRequest request);
    }
}
