namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to provide <see cref="IDatabase"/>s
/// </summary>
public interface IDatabaseProvider
{

    /// <summary>
    /// Gets the provided <see cref="IDatabase"/>
    /// </summary>
    /// <returns>A new <see cref="IDatabase"/></returns>
    IDatabase GetDatabase();

}