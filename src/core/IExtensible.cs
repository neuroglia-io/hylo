namespace Hylo;

/// <summary>
/// Defines the fundamentals of an extensible object
/// </summary>
public interface IExtensible
{

    /// <summary>
    /// Gets an <see cref="IDictionary{TKey, TValue}"/> containing the object's extension data
    /// </summary>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    IDictionary<string, object>? ExtensionData { get; }

}
