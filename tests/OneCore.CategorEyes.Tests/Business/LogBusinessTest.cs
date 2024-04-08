using OneCore.CategorEyes.Business.Log;
using System.Linq.Expressions;

namespace OneCore.CategorEyes.Tests.Business
{
    public class LogBusinessTest
    {

        [Fact]
        public async Task ValidRequest_ReturnsPagedResponse()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var request = new LogRequest { Skip = 0, Take = 10 };
            var expectedHistoricals = new List<Historical>
            {
                new Historical { Id = 1, Description = "Test 1" }
            };
            var expectedTotalPages = expectedHistoricals.Count;

            mockUnitOfWork.Setup(u => u.HistoricalRepository.GetPagedAsync(
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Historical, bool>>>(), It.IsAny<SortDescriptor>(), true))
                .ReturnsAsync((expectedHistoricals, expectedTotalPages));

            var logBusiness = new LogBusiness(mockUnitOfWork.Object);

            // Act
            var response = await logBusiness.GetPaged(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(expectedTotalPages, response.TotalPages);
            Assert.Equal(expectedHistoricals.Count, response.Historicals.Count);
        }

        [Fact]
        public async Task InvalidRequest_SkipNumber_ThrowsException()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var request = new LogRequest { Skip = -1, Take = 10 }; // Invalid skip value

            var logBusiness = new LogBusiness(mockUnitOfWork.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => logBusiness.GetPaged(request));
            Assert.Equal("Skip must be greater than or equal to 0.", exception.Message);
        }

        [Fact]
        public async Task InvalidRequest_TakeNumber_ThrowsException()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var request = new LogRequest { Skip = 1, Take = -10 }; // Invalid skip value

            var logBusiness = new LogBusiness(mockUnitOfWork.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => logBusiness.GetPaged(request));
            Assert.Equal("Take must be greater than or equal to 0.", exception.Message);
        }

        [Fact]
        public async Task EmptyFilter_ReturnsAll()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var request = new LogRequest { Skip = 0, Take = 10, Filter = string.Empty };

            // Assuming you have a setup to return all historicals when the filter is empty
            var allHistoricals = new List<Historical> { /* Populate with test data */ };
            mockUnitOfWork.Setup(u => u.HistoricalRepository.GetPagedAsync(
                    It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<SortDescriptor>(), true))
                .ReturnsAsync((allHistoricals, allHistoricals.Count));

            var logBusiness = new LogBusiness(mockUnitOfWork.Object);

            // Act
            var response = await logBusiness.GetPaged(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(allHistoricals.Count, response.Historicals.Count);
        }

        [Fact]
        public async Task ValidFilter_ReturnsFiltered()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var request = new LogRequest { Skip = 0, Take = 10, Filter = "Test" };

            var filteredHistoricals = new List<Historical>
            {
                new Historical { Id = 1, CreationDate = DateTime.Now, HistoricalType = 1, Description = "Test" },
                new Historical { Id = 3, CreationDate = DateTime.Now, HistoricalType = 3, Description = "Test" },
            };

            mockUnitOfWork.Setup(u => u.HistoricalRepository.GetPagedAsync(
                   It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Historical, bool>>>(), It.IsAny<SortDescriptor>(), true))
               .ReturnsAsync((filteredHistoricals, filteredHistoricals.Count));

            var logBusiness = new LogBusiness(mockUnitOfWork.Object);

            // Act
            var response = await logBusiness.GetPaged(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(filteredHistoricals.Count, response.Historicals.Count);
        }

        [Fact]
        public async Task GetPaged_CallsHistoricalRepositoryOnce()
        {
            // Arrange
            var request = new LogRequest { Skip = 0, Take = 10 };
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockHistoricalRepository = new Mock<IHistoricalRepository>();

            mockUnitOfWork.SetupGet(u => u.HistoricalRepository).Returns(mockHistoricalRepository.Object);
            mockHistoricalRepository.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Historical, bool>>>(), It.IsAny<SortDescriptor>(), true))
                .ReturnsAsync((new List<Historical>(), 0));

            var logBusiness = new LogBusiness(mockUnitOfWork.Object);

            // Act
            await logBusiness.GetPaged(request);

            // Assert
            mockHistoricalRepository.Verify(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Historical, bool>>>(), It.IsAny<SortDescriptor>(), true), Times.Once);
        }

        [Fact]
        public async Task AddUserInteraction_CallsAddAsyncOnce()
        {
            // Arrange
            var request = new UserInteractionRequest { UserInteractionType = (int)UserAction.FilterHistorical };
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockHistoricalRepository = new Mock<IHistoricalRepository>();

            mockUnitOfWork.SetupGet(u => u.HistoricalRepository).Returns(mockHistoricalRepository.Object);

            var logBusiness = new LogBusiness(mockUnitOfWork.Object);

            // Act
            await logBusiness.AddUserInteraction(request);

            // Assert
            mockHistoricalRepository.Verify(r => r.AddAsync(It.IsAny<Historical>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_CallsGetAllAsyncOnce()
        {
            // Arrange
            var request = new LogRequest();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockHistoricalRepository = new Mock<IHistoricalRepository>();

            mockUnitOfWork.SetupGet(u => u.HistoricalRepository).Returns(mockHistoricalRepository.Object);
            mockHistoricalRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Historical, bool>>>(), It.IsAny<SortDescriptor>()))
                .ReturnsAsync(new List<Historical>());

            var logBusiness = new LogBusiness(mockUnitOfWork.Object);

            // Act
            await logBusiness.GetAll(request);

            // Assert
            mockHistoricalRepository.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<Historical, bool>>>(), It.IsAny<SortDescriptor>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_WithoutFilter_CallsGetAllAsyncWithoutFilter()
        {
            // Arrange
            var request = new LogRequest(); // No filter
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockHistoricalRepository = new Mock<IHistoricalRepository>();

            mockUnitOfWork.SetupGet(u => u.HistoricalRepository).Returns(mockHistoricalRepository.Object);
            mockHistoricalRepository.Setup(r => r.GetAllAsync(null, It.IsAny<SortDescriptor>()))
                .ReturnsAsync(new List<Historical>());

            var logBusiness = new LogBusiness(mockUnitOfWork.Object);

            // Act
            await logBusiness.GetAll(request);

            // Assert
            mockHistoricalRepository.Verify(r => r.GetAllAsync(null, It.IsAny<SortDescriptor>()), Times.Once);
        }

        [Fact]
        public async Task GetPaged_WithEmptyFilter_ReturnsAllRecords()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new LogRequest { Skip = 0, Take = 10, Filter = string.Empty }; // Empty Filter

            var historicals = new List<Historical>
            {
                new Historical { Id = 1, CreationDate = DateTime.Now, HistoricalType = 1, Description = "Test" },
                new Historical { Id = 2, CreationDate = DateTime.Now, HistoricalType = 2, Description = @"{
                      ""DocumentTypeName"": ""Invoice"",
                      ""Data"": ""{\""ClientInfo\"": \""Diego Adolfo Gonzalez Flores, Calle El Molino mz20 INT. lt13, Ciudad de México 09960\"", \""ProviderInfo\"": \""TOTAL PLAY TELECOMUNICACIONES S.A.P.I. DE C.V.\"", \""InvoiceNumber\"": \""0900 0001 2050 9665 3\"", \""Date\"": \""12/01/2024\"", \""Products\"": [{\""ProductName\"": \""Renta Plan Ultra Sónico\"", \""Quantity\"": 1, \""UnitPrice\"": 1248.00, \""Total\"": 1248.00}, {\""ProductName\"": \""Suscripciones, TV Adicional y Películas\"", \""Quantity\"": 1, \""UnitPrice\"": 536.00, \""Total\"": 536.00}, {\""ProductName\"": \""Membresías\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}, {\""ProductName\"": \""Descuentos y Promociones\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}, {\""ProductName\"": \""Totalplay Shop\"", \""Quantity\"": 1, \""UnitPrice\"": 0.00, \""Total\"": 0.00}], \""Total\"": 1785.00}"",
                      ""AdditionalData"": ""El total general coincide con la suma de los totales por producto, por lo que la factura es consistente.""
                    }" },
                new Historical { Id = 3, CreationDate = DateTime.Now, HistoricalType = 3, Description = "Test" },
            };

            mockUnitOfWork.Setup(u => u.HistoricalRepository.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), null, null, true))
                           .ReturnsAsync((historicals, historicals.Count)); // Mock response

            // Act
            var result = await logBusiness.GetPaged(request);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPaged_WithSpecificFilter_ReturnsFilteredRecords()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new LogRequest { Skip = 0, Take = 10, Filter = "specificFilter" };

            mockUnitOfWork.Setup(u => u.HistoricalRepository.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Historical, bool>>>(), null, true))
                           .ReturnsAsync((new List<Historical> { new Historical { Description = "specificFilter" } }, 1)); // Mock response with filtered records

            // Act
            var result = await logBusiness.GetPaged(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Historicals);
            Assert.Contains("specificFilter", result.Historicals.First().Description);
        }

        [Fact]
        public async Task AddUserInteraction_WithValidRequest_AddsInteractionSuccessfully()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new UserInteractionRequest { UserInteractionType = (int)UserAction.EnterAnalysisPage };

            mockUnitOfWork.Setup(u => u.HistoricalRepository.AddAsync(It.IsAny<Historical>()))
                           .Returns(Task.CompletedTask); // Setup the repository to successfully add the interaction

            // Act
            await logBusiness.AddUserInteraction(request);

            // Assert
            mockUnitOfWork.Verify(u => u.HistoricalRepository.AddAsync(It.IsAny<Historical>()), Times.Once); // Verify the AddAsync was called once
        }

        [Fact]
        public async Task GetAll_WithNoParameters_ReturnsAllHistoricals()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new LogRequest(); // Empty request

            mockUnitOfWork.Setup(u => u.HistoricalRepository.GetAllAsync(null, null))
                           .ReturnsAsync(new List<Historical> { new Historical(), new Historical() }); // Mock response with 2 historicals

            // Act
            var result = await logBusiness.GetAll(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Expecting two historical records
        }

        [Fact]
        public async Task GetAll_WithFilter_ReturnsFilteredHistoricals()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new LogRequest { Filter = "FilterValue" }; // Request with filter

            mockUnitOfWork.Setup(u => u.HistoricalRepository.GetAllAsync(It.IsAny<Expression<Func<Historical, bool>>>(), null))
                           .ReturnsAsync(new List<Historical> { new Historical { Description = "FilterValue" } }); // Mock response with filtered records

            // Act
            var result = await logBusiness.GetAll(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Expecting one record that matches the filter
            Assert.Contains("FilterValue", result.First().Description);
        }

        [Fact]
        public async Task AddUserInteraction_InvalidUserAction_ThrowsArgumentException()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new UserInteractionRequest { UserInteractionType = -1 }; // Invalid user action type

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => logBusiness.AddUserInteraction(request));
        }

        [Fact]
        public async Task AddUserInteraction_ValidUserAction_CompletesSuccessfully()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new UserInteractionRequest { UserInteractionType = (int)UserAction.FilterHistorical }; // Valid user action type

            mockUnitOfWork.Setup(u => u.HistoricalRepository.GetAllAsync(It.IsAny<Expression<Func<Historical, bool>>>(), null))
                           .ReturnsAsync(new List<Historical> { });

            mockUnitOfWork.Setup(u => u.CompleteAsync());

            // Act
            await logBusiness.AddUserInteraction(request);

            // Assert
            mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once); // Ensures that changes are committed once
        }

        [Fact]
        public async Task GetPaged_NoHistoricalsFound_ReturnsEmptyList()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new LogRequest { Skip = 0, Take = 10 };

            mockUnitOfWork.Setup(u => u.HistoricalRepository.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), null, null, true))
                           .ReturnsAsync((new List<Historical>(), 0)); // Mock response with no historicals

            // Act
            var result = await logBusiness.GetPaged(request);

            // Assert
            Assert.Empty(result.Historicals);
        }

        [Fact]
        public async Task GetAll_SpecificFilter_ReturnsMatchingHistoricals()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new LogRequest { Filter = "Match" };

            var historicals = new List<Historical> { new Historical { Description = "Match" }, new Historical { Description = "No" } };

            mockUnitOfWork.Setup(u => u.HistoricalRepository.GetAllAsync(It.IsAny<Expression<Func<Historical, bool>>>(), null))
                           .ReturnsAsync(historicals.Where(h => h.Description.Contains(request.Filter)).ToList()); // Mock response with filtered historicals

            // Act
            var result = await logBusiness.GetAll(request);

            // Assert
            Assert.Single(result); // Only one historical should match the filter
            Assert.Equal("Match", result.First().Description);
        }

        [Fact]
        public async Task AddUserInteraction_UserActionSaved_HistoricalRecordAdded()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new UserInteractionRequest { UserInteractionType = (int)UserAction.EnterAnalysisPage };

            mockUnitOfWork.Setup(u => u.HistoricalRepository.AddAsync(It.IsAny<Historical>()));

            // Act
            await logBusiness.AddUserInteraction(request);

            // Assert
            mockUnitOfWork.Verify(u => u.HistoricalRepository.AddAsync(It.Is<Historical>(h => h.HistoricalType == (int)HistoricalType.UserInteraction)), Times.Once); // Verify that a historical record of type UserInteraction is added
        }

        [Fact]
        public async Task GetPaged_InvalidSortProperty_ThrowsArgumentException()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var logBusiness = new LogBusiness(mockUnitOfWork.Object);
            var request = new LogRequest
            {
                Skip = 0,
                Take = 10,
                Sort = new SortDescriptor { Property = "InvalidPropertyName", SortOrder = SortOrder.Ascending }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => logBusiness.GetPaged(request));
            Assert.Contains("Invalid property name", exception.Message);
        }
    }
}
