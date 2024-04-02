using OneCore.CategorEyes.Commons.Entities.Common;
using OneCore.CategorEyes.Commons.Requests;
using System.Linq.Expressions;

namespace OneCore.CategorEyes.Business.Persistence.Repositories
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Obtiene todos los elementos de tipo T de la base de datos. Puede filtrarlos mediante un predicado y ordenarlos según un descriptor de ordenación.
        /// </summary>
        /// <param name="predicate">Un predicado opcional para filtrar los elementos, de tipo <see cref="Expression{Func{T, bool}}"/>.</param>
        /// <param name="sortDescriptor">Un descriptor opcional de ordenación para ordenar los resultados, de tipo <see cref="SortDescriptor"/>.</param>
        /// <returns>Una tarea que representa la operación asincrónica y retorna una lista de solo lectura de elementos de tipo T, de tipo <see cref="IReadOnlyList{T}"/>.</returns>
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
            SortDescriptor? sortDescriptor = null);

        /// <summary>
        /// Obtiene una lista paginada de elementos de tipo T de la base de datos. Puede filtrarlos mediante un predicado, ordenarlos según un descriptor de ordenación y decidir si deshabilitar el seguimiento de las entidades.
        /// </summary>
        /// <param name="skip">Número de elementos a omitir para la paginación, de tipo <see cref="int"/>.</param>
        /// <param name="take">Número de elementos a tomar para la paginación, de tipo <see cref="int"/>.</param>
        /// <param name="predicate">Un predicado opcional para filtrar los elementos, de tipo <see cref="Expression{Func{T, bool}}"/>.</param>
        /// <param name="sortDescriptor">Un descriptor opcional de ordenación para ordenar los resultados, de tipo <see cref="SortDescriptor"/>.</param>
        /// <param name="disableTracking">Indica si se debe deshabilitar el seguimiento de las entidades en el resultado, de tipo <see cref="bool"/>.</param>
        /// <returns>Una tarea que representa la operación asincrónica y retorna una tupla que contiene una lista de solo lectura de elementos de tipo T y el total de elementos, de tipos <see cref="IReadOnlyList{T}"/> y <see cref="int"/>, respectivamente.</returns>
        Task<(IReadOnlyList<T>, int)> GetPagedAsync(int skip, int take, Expression<Func<T, bool>>? predicate = null,
            SortDescriptor? sortDescriptor = null,
            bool disableTracking = true);

        /// <summary>
        /// Añade una nueva entidad de tipo T a la base de datos de manera asincrónica.
        /// </summary>
        /// <param name="entity">La entidad a añadir, de tipo T.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task AddAsync(T entity);
    }
}
