using Microsoft.Extensions.Configuration;
using OneCore.CategorEyes.Business.Log;
using OneCore.CategorEyes.Business.Report;

namespace OneCore.CategorEyes.Tests.Business
{
    public class ReportBusinessTest
    {
        private readonly Mock<IOpenAIService> _openAIServiceMock = new Mock<IOpenAIService>();
        private readonly Mock<IHistoricalRepository> _historicalRepositoryMock = new Mock<IHistoricalRepository>();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IBlobService> _blobServiceMock = new Mock<IBlobService>();
        private readonly Mock<ILogBusiness> _logBusinessMock = new Mock<ILogBusiness>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private const string REPORTS_CONTAINER_URL = "https://example.com/reports/";
        private const string FILES_CONTAINER_URL = "https://example.com/files/";
        private readonly ReportRequest _reportRequest = new()
        {
            Headers = new List<string> { "Id", "CreationDate", "HistoricalType", "URL", "Description" },
        };
        private readonly List<Historical> _historicalData = new()
            {
                new Historical { Id = 1, CreationDate = DateTime.Now, HistoricalType = 1, Description = "Test" },
                new Historical { Id = 2, CreationDate = DateTime.Now, HistoricalType = 2, Description = @"{
                      ""DocumentTypeName"": ""Invoice"",
                      ""Data"": ""{\""ClientInfo\"": \""Diego Adolfo Gonzalez Flores, Calle El Molino mz20 INT. lt13, Ciudad de México 09960\"", \""ProviderInfo\"": \""TOTAL PLAY TELECOMUNICACIONES S.A.P.I. DE C.V.\"", \""InvoiceNumber\"": \""0900 0001 2050 9665 3\"", \""Date\"": \""12/01/2024\"", \""Products\"": [{\""ProductName\"": \""Renta Plan Ultra Sónico\"", \""Quantity\"": 1, \""UnitPrice\"": 1248.00, \""Total\"": 1248.00}, {\""ProductName\"": \""Suscripciones, TV Adicional y Películas\"", \""Quantity\"": 1, \""UnitPrice\"": 536.00, \""Total\"": 536.00}, {\""ProductName\"": \""Membresías\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}, {\""ProductName\"": \""Descuentos y Promociones\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}, {\""ProductName\"": \""Totalplay Shop\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}], \""Total\"": 1785.00}"",
                      ""AdditionalData"": ""El total general coincide con la suma de los totales por producto, por lo que la factura es consistente.""
                    }" },
                new Historical { Id = 3, CreationDate = DateTime.Now, HistoricalType = 3, Description = "Test" },
            };

        private ReportBusiness CreateService()
        {
            _unitOfWorkMock.Setup(x => x.HistoricalRepository).Returns(_historicalRepositoryMock.Object);
            _configurationMock.Setup(c => c[Blob.BLOB_REPORTS_CONTAINER_URL_KEY]).Returns(REPORTS_CONTAINER_URL);
            _configurationMock.Setup(c => c[Blob.BLOB_FILES_CONTAINER_URL_KEY]).Returns(FILES_CONTAINER_URL);

            return new ReportBusiness(
                _logBusinessMock.Object,
                _configurationMock.Object,
                _blobServiceMock.Object);
        }


        [Fact]
        public async Task GenerateReport_SuccessfulReportGeneration_ReturnsCorrectResponse()
        {
            // Arrange
            var service = CreateService();

            var expectedReportName = "report.xlsx";
            var expectedReportUrl = $"{REPORTS_CONTAINER_URL}{expectedReportName}";

            _logBusinessMock.Setup(l => l.GetAll(It.IsAny<LogRequest>())).ReturnsAsync(_historicalData);
            _blobServiceMock.Setup(b => b.UploadFile(It.IsAny<FileUpload>(), true)).ReturnsAsync(expectedReportName);

            // Act
            var response = await service.GenerateReport(_reportRequest);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(expectedReportUrl, response.Url);
        }

        [Fact]
        public async Task GenerateReport_EmptyHistoricalData_GeneratesEmptyReport()
        {
            // Arrange
            var service = CreateService();

            // Act
            var response = await service.GenerateReport(_reportRequest);

            // Assert
            _blobServiceMock.Verify(b => b.UploadFile(It.IsAny<FileUpload>(), true), Times.Once);
        }

        [Fact]
        public async Task GenerateReport_ErrorDuringFileUpload_ThrowsException()
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.GenerateReport(new ReportRequest()));
            Assert.Equal("Error uploading report", exception.Message);
        }

        [Fact]
        public async Task GenerateReport_MissingConfigurationKey_ThrowsException()
        {
            // Arrange
            var logBusinessMock = new Mock<ILogBusiness>();
            var configurationMock = new Mock<IConfiguration>();
            var blobServiceMock = new Mock<IBlobService>();
            logBusinessMock.Setup(l => l.GetAll(It.IsAny<LogRequest>())).ReturnsAsync(new List<Historical>());
            var reportBusiness = new ReportBusiness(logBusinessMock.Object, configurationMock.Object, blobServiceMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => reportBusiness.GenerateReport(new ReportRequest()));

            Assert.Equal("Configuration keys missing", exception.Message);
        }

        [Fact]
        public async Task GenerateReport_ValidRequest_GeneratesReport()
        {
            // Arrange
            var service = CreateService();
            _logBusinessMock.Setup(l => l.GetAll(It.IsAny<LogRequest>())).ReturnsAsync(_historicalData);
            _blobServiceMock.Setup(b => b.UploadFile(It.IsAny<FileUpload>(), true)).ReturnsAsync("example.pdf");

            // Act
            var result = await service.GenerateReport(_reportRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Url); // Ensure the generated report has a URL
        }
    }
}
