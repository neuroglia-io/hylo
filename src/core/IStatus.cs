namespace Hylo;

/// <summary>
/// Defines the fundamentals of an object that can be monitored thanks to a status object
/// </summary>
/// <typeparam name="TStatus">The type of the object's status</typeparam>
public interface IStatus<TStatus>
    where TStatus : class, new()
{

    /// <summary>
    /// Gets the object's status
    /// </summary>
    TStatus? Status { get; }

}