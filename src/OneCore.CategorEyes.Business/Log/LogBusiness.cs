using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Business.Log
{
    internal class LogBusiness : ILogBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int MIN_FILTER_LENGTH = 4;

        public LogBusiness(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LogResponse> GetPaged(LogRequest request)
        {
            (IReadOnlyList<Historical> historicals, int totalPages) = !string.IsNullOrEmpty(request.Filter?.Trim()) && request.Filter.Length >= MIN_FILTER_LENGTH ?
                await _unitOfWork.HistoricalRepository.GetPagedAsync(request.Skip, request.Take, x => x.Description.Contains(request.Filter!)) :
                await _unitOfWork.HistoricalRepository.GetPagedAsync(request.Skip, request.Take);

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
