using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;

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
            (IReadOnlyList<Historical> historicals, int totalPages) = !string.IsNullOrEmpty(request.Filter?.Trim()) ?
                await _unitOfWork.HistoricalRepository.GetPagedAsync(request.Skip, request.Take, x => x.Description.Contains(request.Filter!), request.Sort) :
                await _unitOfWork.HistoricalRepository.GetPagedAsync(request.Skip, request.Take, sortDescriptor: request.Sort);

            return new LogResponse
            {
                Historicals = historicals.ToList(),
                Page = request.Skip,
                PageSize = request.Take,
                TotalPages = totalPages
            };
        }
    }
}
