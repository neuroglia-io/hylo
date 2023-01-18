namespace Hylo;

/// <summary>
/// Defines the fundamentals of an event whenever a resource-related operation has been executed
/// </summary>
public interface IResourceEvent
{

    /// <summary>
    /// Gets the type of the resource event
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Gets the API group the resource that has produced the event belongs to
    /// </summary>
    string Group { get; }

    /// <summary>
    /// Gets the API version the resource that has produced the event belongs to
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the plural form of the type of the resource that has produced the event
    /// </summary>
    string Plural { get; }

    /// <summary>
    /// Gets the resource the event has been produced for
    /// </summary>
    object Resource { get; }

}