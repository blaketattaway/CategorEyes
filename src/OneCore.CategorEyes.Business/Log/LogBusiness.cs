using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using System.Linq.Expressions;

namespace OneCore.CategorEyes.Business.Log
{
    internal class LogBusiness : ILogBusiness
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogBusiness(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        public async Task<IEnumerable<Historical>> GetAll(LogRequest request) =>
            await GetHistoricals(request, (skip, take, filter, sort) =>
                _unitOfWork.HistoricalRepository.GetAllAsync(filter, sort));

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
