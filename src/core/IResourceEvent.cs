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
    /// Gets the resource the event has been produced for
    /// </summary>
    object Resource { get; }

}