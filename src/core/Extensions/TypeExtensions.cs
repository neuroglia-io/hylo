using System.Diagnostics.CodeAnalysis;

namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="Type"/>s
/// </summary>
public static class TypeExtensions
{

    /// <summary>
    /// Attempts to get the custom <see cref="Attribute"/> of the specified type
    /// </summary>
    /// <typeparam name="TAttribute">The type of the custom <see cref="Attribute"/> to get</typeparam>
    /// <param name="type">The type to get the specified <see cref="Attribute"/> of</param>
    /// <param name="attribute">The <see cref="Attribute"/> of the specified type, if any</param>
    /// <returns>A boolean indicating whether or not the type is marked with the specified <see cref="Attribute"/></returns>
    public static bool TryGetCustomAttribute<TAttribute>(this Type type, [MaybeNullWhen(false)]out TAttribute? attribute)
        where TAttribute : Attribute
    {
        attribute = type.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
        return attribute != null;
    }

    /// <summary>
    /// Attempts to get the custom <see cref="Attribute"/>s of the specified type
    /// </summary>
    /// <typeparam name="TAttribute">The type of the custom <see cref="Attribute"/> to get</typeparam>
    /// <param name="type">The type to get the specified <see cref="Attribute"/> of</param>
    /// <param name="attributes">An <see cref="IEnumerable{T}"/> containing the <see cref="Attribute"/>s of the specified type, if any</param>
    /// <returns>A boolean indicating whether or not the type is marked with the specified <see cref="Attribute"/></returns>
    public static bool TryGetCustomAttributes<TAttribute>(this Type type, [MaybeNullWhen(false)] out IEnumerable<TAttribute>? attributes)
         where TAttribute : Attribute
    {
        attributes = type.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>();
        return attributes.Any();
    }

}