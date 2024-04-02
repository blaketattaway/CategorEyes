using System.Linq.Expressions;
using System.Reflection;

namespace OneCore.CategorEyes.Infrastructure.Extensions
{
    public static class Extensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, nameof(Queryable.OrderBy));
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, nameof(Queryable.OrderByDescending));
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            var (type, lambda) = CreateLambda<T>(property);
            var method = FindQueryableMethod(methodName, typeof(T), type);
            var result = InvokeQueryableMethod(source, lambda, method);
            return (IOrderedQueryable<T>)result;
        }

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

        private static MethodInfo FindQueryableMethod(string methodName, Type entityType, Type propertyType)
        {
            return typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(entityType, propertyType);
        }

        private static object InvokeQueryableMethod<T>(IQueryable<T> source, LambdaExpression lambda, MethodInfo method)
        {
            return method.Invoke(null, new object[] { source, lambda });
        }
    }
}
