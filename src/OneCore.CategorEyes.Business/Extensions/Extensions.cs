using System.Reflection;

namespace OneCore.CategorEyes.Business.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Retrieves a custom attribute of a specified type from an enumeration value.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute to retrieve, must derive from <see cref="System.Attribute"/>.</typeparam>
        /// <param name="enumValue">The enumeration value from which to retrieve the attribute.</param>
        /// <returns>The attribute of type <typeparamref name="TAttribute"/> associated with the enumeration value, or <c>null</c> if no such attribute exists.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="enumValue"/> is <c>null</c>.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if no member information is found for <paramref name="enumValue"/>, which indicates a serious error in the enum declaration.</exception>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }
    }
}
