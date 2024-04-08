using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using System.Linq.Expressions;
using System.Reflection;

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
            try
            {
                var response = await GetHistoricals(request, (skip, take, filter, sort) =>
                    _unitOfWork.HistoricalRepository.GetPagedAsync(skip, take, filter, sort), true);

                return new LogResponse
                {
                    Historicals = response.Item1.ToList(),
                    TotalPages = response.Item2
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <inheritdoc />
        public async Task AddUserInteraction(UserInteractionRequest request)
        {
            if (!Enum.IsDefined(typeof(UserAction), request.UserInteractionType))
            {
                throw new ArgumentException("Invalid user interaction type.");
            }

            Historical historical = new()
            {
                HistoricalType = (int)HistoricalType.UserInteraction,
                CreationDate = DateTime.Now,
                Description = ((UserAction)request.UserInteractionType).GetDescription()
            };

            await _unitOfWork.HistoricalRepository.AddAsync(historical);
            await _unitOfWork.CompleteAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Historical>> GetAll(LogRequest request) =>
            await GetHistoricals(request, (skip, take, filter, sort) =>
                _unitOfWork.HistoricalRepository.GetAllAsync(filter, sort), false);

        /// <summary>
        /// A generic method to fetch historical data based on provided parameters and a fetch method.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response, e.g., a collection of <see cref="Historical"/> or a tuple of a collection and an integer.</typeparam>
        /// <param name="request">The <see cref="LogRequest"/> parameters.</param>
        /// <param name="fetchMethod">The method to fetch historical data, accepting skip, take, filter, and sort parameters.</param>
        /// <param name="isPaged">A flag indicating whether the response is paged or not within a <see cref="bool"/> value.</param>
        /// <returns>A task representing the asynchronous operation, containing the fetched data.</returns>
        /// <exception cref="ArgumentException">Thrown when 'Skip' is less than 0.</exception>
        /// <exception cref="ArgumentException">Thrown when 'Take' is less than 0.</exception>
        /// <exception cref="ArgumentException">Thrown when 'Sort.Property' is not a valid property name of <see cref="Historical"/>.</exception>
        private static async Task<TResponse> GetHistoricals<TResponse>(
            LogRequest request,
            Func<int, int, Expression<Func<Historical, bool>>?, SortDescriptor?, Task<TResponse>> fetchMethod, bool isPaged)
        {
            Expression<Func<Historical, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                filter = historical => historical.Description.Contains(request.Filter);
            }

            if (isPaged && request.Skip < 0)
                throw new ArgumentException("Skip must be greater than or equal to 0.");

            if (isPaged && request.Take < 0)
                throw new ArgumentException("Take must be greater than or equal to 0.");

            if (request.Sort != null && !IsPropertyNameOfClass<Historical>(request.Sort.Property))
                throw new ArgumentException("Invalid property name");

            return await fetchMethod(request.Skip, request.Take, filter, request.Sort);
        }

        /// <summary>
        /// Checks if the specified string is a property name of the given class type.
        /// </summary>
        /// <typeparam name="T">The class type to check.</typeparam>
        /// <param name="propertyName">The string to check if it's a property name.</param>
        /// <returns>true if the string is a property name; otherwise, false.</returns>
        public static bool IsPropertyNameOfClass<T>(string propertyName)
        {
            // Get the Type object representing the class.
            Type classType = typeof(T);

            // Use reflection to get the PropertyInfo object representing the property.
            // If the property is found, GetProperty returns a PropertyInfo object; otherwise, it returns null.
            PropertyInfo propertyInfo = classType.GetProperty(propertyName);

            // Return true if the property exists (propertyInfo is not null); otherwise, false.
            return propertyInfo != null;
        }
    }
}
