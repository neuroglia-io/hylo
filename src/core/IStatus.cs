namespace Hylo;

/// <summary>
/// Defines the fundamentals of an object which's status is described by a dedicated object
/// </summary>
public interface IStatus
    : ISubResource<IStatus>
{

    /// <summary>
    /// Gets the object's status
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "status", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("status"), YamlMember(Order = 2, Alias = "status")]
    object? Status { get; }

}

/// <summary>
/// Defines the fundamentals of an object which's status is described by a dedicated object
/// </summary>
/// <typeparam name="TStatus"></typeparam>
public interface IStatus<TStatus>
    : IStatus, ISubResource<IStatus, TStatus>
    where TStatus : class, new()
{

    /// <summary>
    /// Gets the object's status
    /// </summary>
    new TStatus? Status { get; }

}
