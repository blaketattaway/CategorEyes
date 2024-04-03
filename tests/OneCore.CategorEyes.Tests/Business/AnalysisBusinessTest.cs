namespace OneCore.CategorEyes.Tests.Business
{
    public class AnalysisBusinessTest
    {
        private readonly Mock<IOpenAIService> _openAIServiceMock = new Mock<IOpenAIService>();
        private readonly Mock<IHistoricalRepository> _historicalRepositoryMock = new Mock<IHistoricalRepository>();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IBlobService> _blobServiceMock = new Mock<IBlobService>();
        private readonly Mock<ILogger<AnalysisBusiness>> _loggerMock = new Mock<ILogger<AnalysisBusiness>>();
        private static readonly AnalysisRequest _analysisRequest = new AnalysisRequest { FileTypeName = "application/pdf", FileType = FileType.Pdf, Base64File = File.ReadAllText(@"..\net6.0\Files\base64_pdfexample.txt") };

        private AnalysisBusiness CreateService()
        {
            _unitOfWorkMock.Setup(x => x.HistoricalRepository).Returns(_historicalRepositoryMock.Object);

            return new AnalysisBusiness(
                _openAIServiceMock.Object,
                _unitOfWorkMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Analyze_ValidRequest_ReturnsExpectedResponse()
        {
            // Arrange
            var service = CreateService(); 

            var expectedResponse = new AnalysisResponse { DocumentType = DocumentType.Invoice };

            _openAIServiceMock.Setup(x => x.Analyze(It.IsAny<object>()))
                .ReturnsAsync(new OpenAIAnalysisResponse { choices = new List<Commons.OpenAI.Choice> { 
                    new Commons.OpenAI.Choice { finish_reason = "stop", index = 0, message = new Commons.OpenAI.Message { role = "assistant", content = @"```json
                    {
                      ""DocumentTypeName"": ""Invoice"",
                      ""Data"": ""{\""ClientInfo\"": \""Diego Adolfo Gonzalez Flores, Calle El Molino mz20 INT. lt13, Ciudad de México 09960\"", \""ProviderInfo\"": \""TOTAL PLAY TELECOMUNICACIONES S.A.P.I. DE C.V.\"", \""InvoiceNumber\"": \""0900 0001 2050 9665 3\"", \""Date\"": \""12/01/2024\"", \""Products\"": [{\""ProductName\"": \""Renta Plan Ultra Sónico\"", \""Quantity\"": 1, \""UnitPrice\"": 1248.00, \""Total\"": 1248.00}, {\""ProductName\"": \""Suscripciones, TV Adicional y Películas\"", \""Quantity\"": 1, \""UnitPrice\"": 536.00, \""Total\"": 536.00}, {\""ProductName\"": \""Membresías\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}, {\""ProductName\"": \""Descuentos y Promociones\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}, {\""ProductName\"": \""Totalplay Shop\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}], \""Total\"": 1785.00}"",
                      ""AdditionalData"": ""El total general coincide con la suma de los totales por producto, por lo que la factura es consistente.""
                    }
                    ```" } }
                }});

            _blobServiceMock.Setup(x => x.UploadFile(It.IsAny<FileUpload>(), false))
                .ReturnsAsync("mockFileName.pdf");

            _historicalRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Historical>()));

            // Act
            var result = await service.Analyze(_analysisRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.DocumentType, result.DocumentType);
            // More assertions based on your expectations

            // Verify that expected methods were called
            _openAIServiceMock.Verify(x => x.Analyze(It.IsAny<object>()), Times.Once);
            _blobServiceMock.Verify(x => x.UploadFile(It.IsAny<FileUpload>(), false), Times.Once);
            _unitOfWorkMock.Verify(x => x.HistoricalRepository.AddAsync(It.IsAny<Historical>()), Times.Exactly(2)); // Assuming two calls: one for document upload, one for analysis result
        }

        [Fact]
        public async Task Analyze_ThrowsException_LogsErrorAndRethrows()
        {
            // Arrange
            var service = CreateService();

            _openAIServiceMock.Setup(x => x.Analyze(It.IsAny<object>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.Analyze(_analysisRequest));
            Assert.Equal("Test exception", exception.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test exception")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
