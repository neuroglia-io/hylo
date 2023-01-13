using System.Security.Claims;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to create <see cref="ClaimsPrincipal"/>s
/// </summary>
public interface IUserClaimsPrincipalFactory
{

    /// <summary>
    /// Creates a new <see cref="ClaimsPrincipal"/> for the specified subject
    /// </summary>
    /// <param name="subject">The subject to create a new <see cref="ClaimsPrincipal"/> for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="ClaimsPrincipal"/> for the specified object</returns>
    Task<ClaimsPrincipal?> CreateAsync(string subject, CancellationToken cancellationToken = default);

}
