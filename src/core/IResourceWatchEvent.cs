namespace Hylo;

/// <summary>
/// Defines the fundamentals of an event produced by a watch
/// </summary>
public interface IResourceWatchEvent
{

    /// <summary>
    /// Gets the event's type<para></para>
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "type", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("type"), YamlMember(Order = 1, Alias = "type")]
    ResourceWatchEventType Type { get; }

    /// <summary>
    /// Gets the object that has produced the <see cref="IResourceWatchEvent"/>
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "resource", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("resource"), YamlMember(Order = 2, Alias = "resource")]
    IResource Resource { get; }

}

/// <summary>
/// Defines the fundamentals of an event produced by a watch
/// </summary>
public interface IResourceWatchEvent<TResource>
    : IResourceWatchEvent
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Gets the object that has produced the <see cref="IResourceWatchEvent"/>
    /// </summary>
    new TResource Resource { get; }

}
