namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to control <see cref="V1Resource"/> versions
/// </summary>
public interface IResourceVersionControl
{

    /// <summary>
    /// Converts the specified resource into the storage version
    /// </summary>
    /// <param name="context">The context of the version conversion to perform, if applicable</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The storage compliant version of the converted resource</returns>
    Task<object> ConvertToStorageVersionAsync(ResourceVersioningContext context, CancellationToken cancellationToken = default);

}
