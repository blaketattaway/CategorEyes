using System.Linq.Expressions;
using System.Reflection;

namespace OneCore.CategorEyes.Infrastructure.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Orders an IQueryable collection by the specified property in ascending order.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection to order, of type <see cref="IQueryable{T}"/>.</param>
        /// <param name="property">The property name by which to order the elements, of type <see cref="string"/>.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> representing the ordered collection.</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, nameof(Queryable.OrderBy));
        }

        /// <summary>
        /// Orders an IQueryable collection by the specified property in descending order.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection to order, of type <see cref="IQueryable{T}"/>.</param>
        /// <param name="property">The property name by which to order the elements, of type <see cref="string"/>.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> representing the ordered collection.</returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, nameof(Queryable.OrderByDescending));
        }

        /// <summary>
        /// Applies ordering to an IQueryable collection based on the specified property and order method.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection to order, of type <see cref="IQueryable{T}"/>.</param>
        /// <param name="property">The property name by which to order the elements, of type <see cref="string"/>.</param>
        /// <param name="methodName">The name of the ordering method to apply, of type <see cref="string"/>. Expected values are "OrderBy" or "OrderByDescending".</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> representing the ordered collection.</returns>
        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            var (type, lambda) = CreateLambda<T>(property);
            var method = FindQueryableMethod(methodName, typeof(T), type);
            var result = InvokeQueryableMethod(source, lambda, method);
            return (IOrderedQueryable<T>)result;
        }

        /// <summary>
        /// Creates a lambda expression for the specified property of type T.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="property">The property name for which to create the lambda expression, of type <see cref="string"/>.</param>
        /// <returns>A tuple containing the property type and the lambda expression, of types <see cref="Type"/> and <see cref="LambdaExpression"/>, respectively.</returns>
        private static (Type, LambdaExpression) CreateLambda<T>(string property)
        {
            var props = property.Split('.');
            var type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (var prop in props)
            {
                var pi = type.GetProperty(prop) ?? throw new ArgumentException($"Property '{prop}' not found on type '{type.Name}'.");
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);
            return (type, lambda);
        }

        /// <summary>
        /// Finds the Queryable method that matches the specified method name, source type, and property type.
        /// </summary>
        /// <param name="methodName">The name of the Queryable method to find, of type <see cref="string"/>.</param>
        /// <param name="entityType">The source type, of type <see cref="Type"/>.</param>
        /// <param name="propertyType">The type of the property by which to order, of type <see cref="Type"/>.</param>
        /// <returns>The found Queryable method, of type <see cref="MethodInfo"/>.</returns>
        private static MethodInfo FindQueryableMethod(string methodName, Type entityType, Type propertyType)
        {
            return typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(entityType, propertyType);
        }

        /// <summary>
        /// Invokes the specified Queryable method with the given source collection and lambda expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection to order, of type <see cref="IQueryable{T}"/>.</param>
        /// <param name="lambda">The lambda expression for ordering, of type <see cref="LambdaExpression"/>.</param>
        /// <param name="method">The Queryable method to invoke, of type <see cref="MethodInfo"/>.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> representing the ordered collection.</returns>
        private static object InvokeQueryableMethod<T>(IQueryable<T> source, LambdaExpression lambda, MethodInfo method)
        {
            return method.Invoke(null, new object[] { source, lambda });
        }
    }
}
