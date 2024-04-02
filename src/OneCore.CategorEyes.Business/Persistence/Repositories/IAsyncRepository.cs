using OneCore.CategorEyes.Commons.Entities.Common;
using OneCore.CategorEyes.Commons.Requests;
using System.Linq.Expressions;

namespace OneCore.CategorEyes.Business.Persistence.Repositories
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Retrieves all elements of type T from the database. Can filter them using a predicate and order them according to a sort descriptor.
        /// </summary>
        /// <param name="predicate">An optional predicate to filter elements, of type <see cref="Expression{Func{T, bool}}"/>.</param>
        /// <param name="sortDescriptor">An optional sort descriptor to order the results, of type <see cref="SortDescriptor"/>.</param>
        /// <returns>A task representing the asynchronous operation, returning a read-only list of elements of type T, of type <see cref="IReadOnlyList{T}"/>.</returns>
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
            SortDescriptor? sortDescriptor = null);

        /// <summary>
        /// Retrieves a paginated list of elements of type T from the database. Can filter them using a predicate, order them according to a sort descriptor, and decide whether to disable entity tracking.
        /// </summary>
        /// <param name="skip">The number of elements to skip for pagination, of type <see cref="int"/>.</param>
        /// <param name="take">The number of elements to take for pagination, of type <see cref="int"/>.</param>
        /// <param name="predicate">An optional predicate to filter elements, of type <see cref="Expression{Func{T, bool}}"/>.</param>
        /// <param name="sortDescriptor">An optional sort descriptor to order the results, of type <see cref="SortDescriptor"/>.</param>
        /// <param name="disableTracking">Indicates whether to disable tracking of entities in the result, of type <see cref="bool"/>.</param>
        /// <returns>A task representing the asynchronous operation, returning a tuple containing a read-only list of elements of type T and the total count of elements, of types <see cref="IReadOnlyList{T}"/> and <see cref="int"/>, respectively.</returns>
        Task<(IReadOnlyList<T>, int)> GetPagedAsync(int skip, int take, Expression<Func<T, bool>>? predicate = null,
            SortDescriptor? sortDescriptor = null,
            bool disableTracking = true);

        /// <summary>
        /// Asynchronously adds a new entity of type T to the database.
        /// </summary>
        /// <param name="entity">The entity to add, of type T.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(T entity);
    }
}
