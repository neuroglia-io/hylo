namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to provide the <see cref="IResourceStorage"/> used by <see cref="IResourceRepository"/> implementations to persist <see cref="IResource"/>s
/// </summary>
public interface IResourceStorageProvider
{

    /// <summary>
    /// Gets an <see cref="IResourceStorage"/>
    /// </summary>
    /// <returns>An <see cref="IResourceStorage"/> implementation</returns>
    IResourceStorage GetResourceStorage();

}