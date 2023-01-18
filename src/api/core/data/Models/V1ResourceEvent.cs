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
    /// <param name="type">The <see cref="V1ResourceEvent"/>'s type</param>
    /// <param name="group">The API group the resource that has produced the <see cref="V1ResourceEvent"/> belongs to</param>
    /// <param name="version">The API version the resource that has produced the <see cref="V1ResourceEvent"/> belongs to</param>
    /// <param name="plural">The plural form of the type of the resource that has produced the <see cref="V1ResourceEvent"/></param>
    /// <param name="resource">The resource the <see cref="V1ResourceEvent"/> has been produced for</param>
    public V1ResourceEvent(string type, string group, string version, string plural, object resource)
    {
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        this.Type = type;
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    /// <inheritdoc/>
    [DataMember(Name = "type", Order = 1), JsonPropertyName("type"), Required, MinLength(1)]
    public virtual string Type { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "group", Order = 2), JsonPropertyName("group"), Required, MinLength(1)]
    public virtual string Group { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "version", Order = 3), JsonPropertyName("version"), Required, MinLength(1)]
    public virtual string Version { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "plural", Order = 4), JsonPropertyName("plural"), Required, MinLength(1)]
    public virtual string Plural { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "resource", Order = 5), JsonPropertyName("resource"), Required]
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
    /// <param name="group">The API group the resource that has produced the <see cref="V1ResourceEvent"/> belongs to</param>
    /// <param name="version">The API version the resource that has produced the <see cref="V1ResourceEvent"/> belongs to</param>
    /// <param name="plural">The plural form of the type of the resource that has produced the <see cref="V1ResourceEvent"/></param>
    /// <param name="resource">The resource the <see cref="V1ResourceEvent"/> has been produced for</param>
    public V1ResourceEvent(string type, string group, string version, string plural, TResource resource)
    {
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        this.Type = type;
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    /// <inheritdoc/>
    [DataMember(Name = "type", Order = 1), JsonPropertyName("type"), Required, MinLength(1)]
    public virtual string Type { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "group", Order = 2), JsonPropertyName("group"), Required, MinLength(1)]
    public virtual string Group { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "version", Order = 3), JsonPropertyName("version"), Required, MinLength(1)]
    public virtual string Version { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "plural", Order = 4), JsonPropertyName("plural"), Required, MinLength(1)]
    public virtual string Plural { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "resource", Order = 5), JsonPropertyName("resource"), Required]
    public virtual TResource Resource { get; set; } = null!;

    object IResourceEvent.Resource => this.Resource;

}