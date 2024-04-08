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
        private static readonly AnalysisRequest _emptyPDFRequest = new AnalysisRequest { FileTypeName = "application/pdf", FileType = FileType.Pdf, Base64File = File.ReadAllText(@"..\net6.0\Files\base64_pdfwithnotext.txt") };

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
                .ReturnsAsync(new OpenAIAnalysisResponse
                {
                    choices = new List<Commons.OpenAI.Choice> {
                    new Commons.OpenAI.Choice { finish_reason = "stop", index = 0, message = new Commons.OpenAI.Message { role = "assistant", content = @"```json
                    {
                      ""DocumentTypeName"": ""Invoice"",
                      ""Data"": ""{\""ClientInfo\"": \""Diego Adolfo Gonzalez Flores, Calle El Molino mz20 INT. lt13, Ciudad de México 09960\"", \""ProviderInfo\"": \""TOTAL PLAY TELECOMUNICACIONES S.A.P.I. DE C.V.\"", \""InvoiceNumber\"": \""0900 0001 2050 9665 3\"", \""Date\"": \""12/01/2024\"", \""Products\"": [{\""ProductName\"": \""Renta Plan Ultra Sónico\"", \""Quantity\"": 1, \""UnitPrice\"": 1248.00, \""Total\"": 1248.00}, {\""ProductName\"": \""Suscripciones, TV Adicional y Películas\"", \""Quantity\"": 1, \""UnitPrice\"": 536.00, \""Total\"": 536.00}, {\""ProductName\"": \""Membresías\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}, {\""ProductName\"": \""Descuentos y Promociones\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}, {\""ProductName\"": \""Totalplay Shop\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}], \""Total\"": 1785.00}"",
                      ""AdditionalData"": ""El total general coincide con la suma de los totales por producto, por lo que la factura es consistente.""
                    }
                    ```" } }
                }
                });

            _blobServiceMock.Setup(x => x.UploadFile(It.IsAny<FileUpload>(), false))
                .ReturnsAsync("mockFileName.pdf");

            _historicalRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Historical>()));

            // Act
            var result = await service.Analyze(_analysisRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.DocumentType, result.DocumentType);

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

            _blobServiceMock.Setup(x => x.UploadFile(It.IsAny<FileUpload>(), false))
                .ReturnsAsync("mockFileName.pdf");

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

        [Fact]
        public async Task Analyze_UnsupportedFileType_ThrowsArgumentException()
        {
            // Arrange
            var service = CreateService();
            var unsupportedFileTypeRequest = new AnalysisRequest { FileTypeName = "image/gif", FileType = FileType.Unknown, Base64File = "base64string" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.Analyze(unsupportedFileTypeRequest));
            Assert.Equal("Unknown file type", exception.Message);

            // Verify that no methods were called due to early termination
            _openAIServiceMock.Verify(x => x.Analyze(It.IsAny<object>()), Times.Never);
            _blobServiceMock.Verify(x => x.UploadFile(It.IsAny<FileUpload>(), false), Times.Never);
            _unitOfWorkMock.Verify(x => x.HistoricalRepository.AddAsync(It.IsAny<Historical>()), Times.Never);
        }

        [Fact]
        public async Task Analyze_EmptyBase64File_ThrowsArgumentException()
        {
            // Arrange
            var service = CreateService();
            var emptyBase64Request = new AnalysisRequest { FileTypeName = "application/pdf", FileType = FileType.Pdf, Base64File = "" };

            _blobServiceMock.Setup(x => x.UploadFile(It.IsAny<FileUpload>(), false)).ReturnsAsync(string.Empty);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.Analyze(emptyBase64Request));
            Assert.Equal("Base64File cannot be null", exception.Message);

            // Verify no interactions due to validation failure
            _openAIServiceMock.VerifyNoOtherCalls();
            _blobServiceMock.VerifyNoOtherCalls();
            _unitOfWorkMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Analyze_CorruptBase64File_ThrowsFormatException()
        {
            // Arrange
            var service = CreateService();
            var corruptBase64Request = new AnalysisRequest { FileTypeName = "application/pdf", FileType = FileType.Pdf, Base64File = "not_base64" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.Analyze(corruptBase64Request));
            Assert.Equal("Invalid base64 string", exception.Message);

            // Verify no interactions due to format exception
            _openAIServiceMock.VerifyNoOtherCalls();
            _blobServiceMock.VerifyNoOtherCalls();
            _unitOfWorkMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Analyze_OpenAIReturnsEmptyChoices_ThrowsException()
        {
            // Arrange
            var service = CreateService();
            _openAIServiceMock.Setup(x => x.Analyze(It.IsAny<object>())).ReturnsAsync(new OpenAIAnalysisResponse { choices = new List<Commons.OpenAI.Choice>() });
            _blobServiceMock.Setup(x => x.UploadFile(It.IsAny<FileUpload>(), false))
                .ReturnsAsync("mockFileName.pdf");
            _historicalRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Historical>()));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Analyze(_analysisRequest));

            // Verify interactions up to OpenAI service call
            _openAIServiceMock.Verify(x => x.Analyze(It.IsAny<object>()), Times.Once);
            _blobServiceMock.Verify(x => x.UploadFile(It.IsAny<FileUpload>(), false), Times.Once);
            _unitOfWorkMock.Verify(x => x.HistoricalRepository.AddAsync(It.IsAny<Historical>()), Times.Once); // For document upload log
        }

        [Fact]
        public async Task Analyze_BlobServiceUploadFails_ThrowsException()
        {
            // Arrange
            var service = CreateService();
            _blobServiceMock.Setup(x => x.UploadFile(It.IsAny<FileUpload>(), false)).ThrowsAsync(new Exception("Blob upload failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Analyze(_analysisRequest));

            // Verify interactions up to blob service call
            _blobServiceMock.Verify(x => x.UploadFile(It.IsAny<FileUpload>(), false), Times.Once);
            _openAIServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Analyze_HistoricalRepositoryAddFails_ThrowsException()
        {
            // Arrange
            var service = CreateService();
            _blobServiceMock.Setup(x => x.UploadFile(It.IsAny<FileUpload>(), false))
                .ReturnsAsync("mockFileName.pdf");
            _historicalRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Historical>())).ThrowsAsync(new Exception("AddAsync failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Analyze(_analysisRequest));

            // Verify interactions up to the first AddAsync call
            _blobServiceMock.Verify(x => x.UploadFile(It.IsAny<FileUpload>(), false), Times.Once);
            _historicalRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Historical>()), Times.Once);
        }

        [Fact]
        public async Task Analyze_OpenAIServiceThrowsException_LogsAndRethrows()
        {
            // Arrange
            var service = CreateService();
            _openAIServiceMock.Setup(x => x.Analyze(It.IsAny<object>())).ThrowsAsync(new Exception("OpenAI service exception"));
            _blobServiceMock.Setup(x => x.UploadFile(It.IsAny<FileUpload>(), false))
               .ReturnsAsync("mockFileName.pdf");
            _historicalRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Historical>()));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Analyze(_analysisRequest));

            // Verify interaction up to the OpenAI service call
            _openAIServiceMock.Verify(x => x.Analyze(It.IsAny<object>()), Times.Once);

            // Verify logging
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("OpenAI service exception")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Analyze_ValidPdfRequestWithNoText_ThrowsCustomException()
        {
            // Arrange
            var service = CreateService();
            _blobServiceMock.Setup(x => x.UploadFile(It.IsAny<FileUpload>(), false))
               .ReturnsAsync("mockFileName.pdf");
            _historicalRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Historical>()));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.Analyze(_emptyPDFRequest));

            // Verify the exception message or type as needed
            Assert.Equal("The pdf has no text to send", exception.Message);

            // Verify that expected methods were called
            _openAIServiceMock.Verify(x => x.Analyze(It.IsAny<object>()), Times.Never);
            _blobServiceMock.Verify(x => x.UploadFile(It.IsAny<FileUpload>(), false), Times.Once);
            _historicalRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Historical>()), Times.Once);
        }
    }
}
