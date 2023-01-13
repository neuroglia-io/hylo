namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents an event fired whenever a <see cref="V1Resource"/> has been created, updated or deleted
/// </summary>
[DataContract]
public class V1ResourceEvent
    : IResourceEvent
{

    /// <summary>
    /// Initializes a new <see cref="V1ResourceEvent"/>
    /// </summary>
    public V1ResourceEvent() { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceEvent"/>
    /// </summary>
    /// <param name="type">The event's type</param>
    /// <param name="resource">The resource the <see cref="V1ResourceEvent"/> has been produced for</param>
    public V1ResourceEvent(string type, object resource)
    {
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    /// <inheritdoc/>
    [DataMember(Name = "type", Order = 1), JsonPropertyName("type"), Required, MinLength(1)]
    public virtual string Type { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "resource", Order = 2), JsonPropertyName("resource"), Required]
    public virtual object Resource { get; set; } = null!;

}

/// <summary>
/// Represents an event fired whenever a <see cref="V1Resource"/> has been created, updated or deleted
/// </summary>
/// <typeparam name="TResource">The type of <see cref="V1Resource"/> the event has been produced for</typeparam>
[DataContract]
public class V1ResourceEvent<TResource>
    : IResourceEvent
    where TResource : V1Resource
{

    /// <summary>
    /// Initializes a new <see cref="V1ResourceEvent"/>
    /// </summary>
    public V1ResourceEvent() { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceEvent"/>
    /// </summary>
    /// <param name="type">The event's type</param>
    /// <param name="resource">The resource the <see cref="V1ResourceEvent"/> has been produced for</param>
    public V1ResourceEvent(string type, TResource resource)
    {
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    /// <inheritdoc/>
    [DataMember(Name = "type", Order = 1), JsonPropertyName("type"), Required, MinLength(1)]
    public virtual string Type { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "resource", Order = 2), JsonPropertyName("resource"), Required]
    public virtual TResource Resource { get; set; } = null!;

    object IResourceEvent.Resource => this.Resource;

}