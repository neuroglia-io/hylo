using System.Security.Claims;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to authorize users to operate on resources
/// </summary>
public interface IResourceAccessControl
{

    /// <summary>
    /// Authorizes a user to perform a specific operation on a resource
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> to authorize</param>
    /// <param name="verb">The verb that describes the operation to perform</param>
    /// <param name="group">The API group of the resource to operate on</param>
    /// <param name="apiVersion">The version of the API the resource to operate on belongs to</param>
    /// <param name="pluralName">The plural name of the type of the resource to operate on</param>
    /// <param name="name">The name of the resource to operate on</param>
    /// <param name="namespace">The namespace of the resource to operate on, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task<bool> AuthorizeResourceAccessAsync(ClaimsPrincipal user, string verb, string group, string apiVersion, string pluralName, string? name = null, string? @namespace = null, CancellationToken cancellationToken = default);

}