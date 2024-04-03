using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;

namespace OneCore.CategorEyes.Business.Report
{
    public interface IReportBusiness
    {
        Task<ReportResponse> GenerateReport(ReportRequest request);
    }
}
