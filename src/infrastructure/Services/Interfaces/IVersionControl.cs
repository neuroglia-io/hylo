namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to control <see cref="IResource"/> versions
/// </summary>
public interface IVersionControl
{

    /// <summary>
    /// Converts the specified resource into the storage version
    /// </summary>
    /// <param name="context">The context of the version conversion to perform, if applicable</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The storage compliant version of the converted resource</returns>
    Task<IResource> ConvertToStorageVersionAsync(VersioningContext context, CancellationToken cancellationToken = default);

}
