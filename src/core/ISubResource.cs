namespace Hylo;

/// <summary>
/// Defines the fundamentals of a resource that defines sub resource
/// </summary>
public interface ISubResource
{



}

/// <summary>
/// Defines the fundamentals of a resource that defines sub resource
/// </summary>
/// <typeparam name="TContainer">The type of the sub resource container</typeparam>
public interface ISubResource<TContainer>
    : ISubResource
    where TContainer : class
{

    /// <summary>
    /// Gets the sub resource, if any
    /// </summary>
    object? SubResource { get; }

}

/// <summary>
/// Defines the fundamentals of a resource that defines sub resource
/// </summary>
/// <typeparam name="TContainer">The type of the sub resource container</typeparam>
/// <typeparam name="TSubResource">The type of the sub resource</typeparam>
public interface ISubResource<TContainer, TSubResource>
    : ISubResource<TContainer>
    where TContainer : class
{

    /// <summary>
    /// Gets the sub resource, if any
    /// </summary>
    new TSubResource? SubResource { get; }

}