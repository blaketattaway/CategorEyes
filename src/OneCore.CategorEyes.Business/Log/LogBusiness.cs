using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using System.Linq.Expressions;

namespace OneCore.CategorEyes.Business.Log
{
    /// <summary>
    /// Provides business logic for logging and retrieving historical data.
    /// </summary>
    public class LogBusiness : ILogBusiness
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogBusiness"/> class with a unit of work for database operations.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/> for database operations.</param>
        public LogBusiness(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<LogResponse> GetPaged(LogRequest request)
        {
            var response = await GetHistoricals(request, (skip, take, filter, sort) =>
                _unitOfWork.HistoricalRepository.GetPagedAsync(skip, take, filter, sort));

            return new LogResponse
            {
                Historicals = response.Item1.ToList(),
                TotalPages = response.Item2
            };
        }

        /// <inheritdoc />
        public async Task AddUserInteraction(UserInteractionRequest request)
        {
            Historical historical = new ()
            {
                HistoricalType = (int)HistoricalType.UserInteraction,
                CreationDate = DateTime.Now,
                Description = ((UserActions)request.UserInteractionType).GetDescription()
            };

            await _unitOfWork.HistoricalRepository.AddAsync(historical);
            await _unitOfWork.CompleteAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Historical>> GetAll(LogRequest request) =>
            await GetHistoricals(request, (skip, take, filter, sort) =>
                _unitOfWork.HistoricalRepository.GetAllAsync(filter, sort));

        /// <summary>
        /// A generic method to fetch historical data based on provided parameters and a fetch method.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response, e.g., a collection of <see cref="Historical"/> or a tuple of a collection and an integer.</typeparam>
        /// <param name="request">The <see cref="LogRequest"/> parameters.</param>
        /// <param name="fetchMethod">The method to fetch historical data, accepting skip, take, filter, and sort parameters.</param>
        /// <returns>A task representing the asynchronous operation, containing the fetched data.</returns>
        private static async Task<TResponse> GetHistoricals<TResponse>(
            LogRequest request,
            Func<int, int, Expression<Func<Historical, bool>>?, SortDescriptor?, Task<TResponse>> fetchMethod)
        {
            Expression<Func<Historical, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                filter = historical => historical.Description.Contains(request.Filter);
            }

            return await fetchMethod(request.Skip, request.Take, filter, request.Sort);
        }
    }
}
