using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OneCore.CategorEyes.Business.Extensions;
using OneCore.CategorEyes.Business.Log;
using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Business.Services;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace OneCore.CategorEyes.Business.Report
{
    /// <summary>
    /// Responsible for generating and managing reports related to the application's logging system.
    /// </summary>
    public class ReportBusiness : IReportBusiness
    {
        private readonly ILogBusiness _logBusiness;
        private readonly IConfiguration _configuration;
        private readonly IBlobService _blobService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportBusiness"/> class with the necessary services.
        /// </summary>
        /// <param name="logBusiness">The log business service used to retrieve log entries.</param>
        /// <param name="configuration">The configuration service for accessing application settings.</param>
        /// <param name="blobService">The blob service used for storing and retrieving report files.</param>
        public ReportBusiness(ILogBusiness logBusiness, IConfiguration configuration, IBlobService blobService)
        {
            _logBusiness = logBusiness;
            _configuration = configuration;
            _blobService = blobService;
        }

        /// <inheritdoc />
        public async Task<ReportResponse> GenerateReport(ReportRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(_configuration[Blob.BLOB_FILES_CONTAINER_URL_KEY]) || string.IsNullOrEmpty(_configuration[Blob.BLOB_REPORTS_CONTAINER_URL_KEY]))
                    throw new KeyNotFoundException("Configuration keys missing");

                var reportByteArray = await GenerateExcel(request);

                await _logBusiness.AddUserInteraction(new UserInteractionRequest
                {
                    UserInteractionType = (int)UserAction.ExportHistorical,
                });

                var reportName = await _blobService.UploadFile(new Commons.Blob.FileUpload
                {
                    Base64File = Convert.ToBase64String(reportByteArray),
                    Extension = "xlsx",
                    ContentType = ContentType.APPLICATION_XLSX,
                }, true);

                return new() { Url = $"{_configuration[Blob.BLOB_REPORTS_CONTAINER_URL_KEY]}{reportName}" };
            }
            catch (KeyNotFoundException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new Exception("Error uploading report");
            }
        }

        /// <summary>
        /// Generates the Excel report content as a byte array.
        /// </summary>
        /// <param name="request">The <see cref="ReportRequest"/> containing parameters for the report generation.</param>
        /// <returns>A <see cref="Task{byte[]}"/> representing the asynchronous operation, containing the generated Excel report as a byte array.</returns>
        private async Task<byte[]> GenerateExcel(ReportRequest request)
        {
            try
            {
                using ExcelPackage excelPackage = new();

                var historicals = await _logBusiness.GetAll(new LogRequest
                {
                    Sort = request.Sort,
                    Filter = request.Filter,
                });

                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Historicals");

                AddHeaders(request.Headers, worksheet);
                AddData(historicals, worksheet);

                return excelPackage.GetAsByteArray();
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error generating report");
            }
        }

        /// <summary>
        /// Adds headers to the Excel worksheet.
        /// </summary>
        /// <param name="headers">A <see cref="List{string}"/> representing the headers to be added to the worksheet.</param>
        /// <param name="worksheet">The <see cref="ExcelWorksheet"/> where headers will be added.</param>
        private static void AddHeaders(List<string> headers, ExcelWorksheet worksheet)
        {
            int row = 1;
            int column = 1;

            foreach (string header in headers)
            {
                worksheet.Cells[row, column++].Value = header;
            }

            worksheet.Cells[1, 1, 1, column].Style.Font.Bold = true;
        }

        /// <summary>
        /// Adds historical data to the Excel worksheet.
        /// </summary>
        /// <param name="historicals">An <see cref="IEnumerable{Historical}"/> containing historical data to be added to the worksheet.</param>
        /// <param name="worksheet">The <see cref="ExcelWorksheet"/> where historical data will be added.</param>
        private void AddData(IEnumerable<Historical> historicals, ExcelWorksheet worksheet)
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

            StyleWorksheet(worksheet, historicals.Count() + 1, column);
        }

        /// <summary>
        /// Applies styling to the Excel worksheet.
        /// </summary>
        /// <param name="worksheet">The <see cref="ExcelWorksheet"/> to be styled.</param>
        /// <param name="rowCount">The number of rows in the worksheet, including headers.</param>
        /// <param name="columnCount">The number of columns in the worksheet.</param>
        private static void StyleWorksheet(ExcelWorksheet worksheet, int rowCount, int columnCount)
        {
            worksheet.Cells[1, 1, rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1, rowCount, columnCount].AutoFitColumns();
        }
    }
}
