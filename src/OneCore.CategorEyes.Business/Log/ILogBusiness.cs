using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;

namespace OneCore.CategorEyes.Business.Log
{
    public interface ILogBusiness
    {
        /// <summary>
        /// Retrieves a paginated list of <see cref="Historical"/> entries based on the provided <see cref="LogRequest"/>.
        /// </summary>
        /// <param name="request">The <see cref="LogRequest"/> containing pagination and filter information.</param>
        /// <returns>A task representing the asynchronous operation, containing the <see cref="LogResponse"/> with paginated log entries.</returns>
        Task<LogResponse> GetPaged(LogRequest request);

        /// <summary>
        /// Adds a new <see cref="Historical"/> entry for user interaction to the database.
        /// </summary>
        /// <param name="request">The <see cref="UserInteractionRequest"/> containing user interaction data.</param>
        /// <returns>A task representing the asynchronous operation to add the log entry.</returns>
        Task AddUserInteraction(UserInteractionRequest request);

        /// <summary>
        /// Retrieves all <see cref="Historical"/> entries matching the provided <see cref="LogRequest"/>.
        /// </summary>
        /// <param name="request">The <see cref="LogRequest"/> containing filter information.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of <see cref="Historical"/> entries.</returns>
        Task<IEnumerable<Historical>> GetAll(LogRequest request);
    }
}
