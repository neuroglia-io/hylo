namespace Hylo;

/// <summary>
/// Defines the fundamentals of an extensible object
/// </summary>
public interface IExtensible
{

    /// <summary>
    /// Gets an <see cref="IDictionary{TKey, TValue}"/> containing the object's extensions
    /// </summary>
    IDictionary<string, object>? Extensions { get; set; }

}