namespace Hylo;

/// <summary>
/// Defines the fundamentals of an object that exposes specifications
/// </summary>
/// <typeparam name="TSpec">The type of the object's spec</typeparam>
public interface ISpec<TSpec>
    where TSpec : class, new()
{

    /// <summary>
    /// Gets the object's spec
    /// </summary>
    TSpec Spec { get; }

}
