namespace Hylo.Api.Configuration;

/// <summary>
/// Defines the fundamentals of a service used to register well-known <see cref="V1Resource"/>s
/// </summary>
public interface IResourceRegistry
{

    /// <summary>
    /// Registers the specified <see cref="V1Resource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="V1Resource"/> to register</typeparam>
    /// <param name="resource">The <see cref="V1Resource"/> to register</param>
    void Register<TResource>(TResource resource)
        where TResource : V1Resource, new();

}