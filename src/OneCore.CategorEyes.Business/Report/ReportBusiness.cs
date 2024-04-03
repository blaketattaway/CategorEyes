using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OneCore.CategorEyes.Business.Extensions;
using OneCore.CategorEyes.Business.Log;
using OneCore.CategorEyes.Business.Services;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace OneCore.CategorEyes.Business.Report
{
    public class ReportBusiness : IReportBusiness
    {
        private readonly ILogBusiness _logBusiness;
        private readonly IConfiguration _configuration;
        private readonly IBlobService _blobService;

        public ReportBusiness(ILogBusiness logBusiness, IConfiguration configuration, IBlobService blobService)
        {
            _logBusiness = logBusiness;
            _configuration = configuration;
            _blobService = blobService;
        }

        public async Task<ReportResponse> GenerateReport(ReportRequest request)
        {
            var reportByteArray = await GenerateExcel(request);

            var reportName = await _blobService.UploadFile(new Commons.Blob.FileUpload
            {
                Base64File = Convert.ToBase64String(reportByteArray),
                Extension = "xlsx",
                ContentType = ContentType.APPLICATION_XLSX,
            }, true);

            return new() { Url = $"{_configuration[Blob.BLOB_REPORTS_CONTAINER_URL_KEY]}{reportName}" };
        }

        private async Task<byte[]> GenerateExcel(ReportRequest request)
        {
            try
            {

                IEnumerable<Historical> historicals = await _logBusiness.GetAll(new LogRequest
                {
                    Sort = request.Sort,
                    Filter = request.Filter,
                });

                using ExcelPackage excelPackage = new();

                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Historicals");

                AddHeaders(request.Headers, ref worksheet);

                AddData(historicals, ref worksheet);

                return excelPackage.GetAsByteArray();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void AddHeaders(List<string> headers, ref ExcelWorksheet worksheet)
        {
            int row = 1;
            int column = 1;

            foreach (string header in headers)
            {
                worksheet.Cells[row, column++].Value = header;
            }

            worksheet.Cells[1, 1, 1, column].Style.Font.Bold = true;
        }

        private void AddData(IEnumerable<Historical> historicals, ref ExcelWorksheet worksheet)
        {
            int row = 2;
            int column = 1;
            foreach (Historical historical in historicals)
            {
                column = 1;
                worksheet.Cells[row, column++].Value = historical.Id;
                worksheet.Cells[row, column++].Value = ((HistoricalType)historical.HistoricalType).GetAttribute<DisplayAttribute>()?.Name ?? string.Empty;
                worksheet.Cells[row, column++].Value = $"{historical.CreationDate:dd/MM/yyyy HH:mm}";
                switch ((HistoricalType)historical.HistoricalType)
                {
                    case HistoricalType.DocumentUpload:
                        var cellDocumentUpload = worksheet.Cells[row, column++];
                        cellDocumentUpload.Hyperlink = new Uri($"{_configuration[Blob.BLOB_FILES_CONTAINER_URL_KEY]}{historical.Description}");
                        cellDocumentUpload.Value = "URL";
                        cellDocumentUpload.Style.Font.UnderLine = true;
                        worksheet.Cells[row, column++].Value = string.Empty;
                        break;
                    case HistoricalType.IA:
                        var analysisResponse = JsonSerializer.Deserialize<AnalysisResponse>(historical.Description);
                        var cellIA = worksheet.Cells[row, column++];
                        cellIA.Hyperlink = new Uri($"{_configuration[Blob.BLOB_FILES_CONTAINER_URL_KEY]}{analysisResponse.FileName}");
                        cellIA.Value = "URL";
                        cellIA.Style.Font.UnderLine = true;
                        worksheet.Cells[row, column++].Value = $"Data: { analysisResponse.Data } AdditionalData: { analysisResponse.AdditionalData }";
                        break;
                    case HistoricalType.UserInteraction:
                        worksheet.Cells[row, column++].Value = string.Empty;
                        worksheet.Cells[row, column++].Value = historical.Description;
                        break;
                    default:
                        worksheet.Cells[row, column++].Value = string.Empty;
                        worksheet.Cells[row, column++].Value = string.Empty;
                        break;
                }
                row++;
            }

            worksheet.Cells[1, 1, historicals.Count() + 1, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1, historicals.Count() + 1, column - 1].AutoFitColumns();
        }
    }
}
