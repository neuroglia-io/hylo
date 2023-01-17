using System.Security.Claims;

namespace Hylo.Api.Authorization.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to create <see cref="ClaimsPrincipal"/>s
/// </summary>
public interface IIdentityClaimsPrincipalFactory
{

    /// <summary>
    /// Creates a new <see cref="ClaimsPrincipal"/> for the specified subject
    /// </summary>
    /// <param name="subject">The subject to create a new <see cref="ClaimsPrincipal"/> for</param>
    /// <param name="authenticationType">The authentication type that has been used to authenticate the subject</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="ClaimsPrincipal"/> for the specified object</returns>
    Task<ClaimsPrincipal?> CreateAsync(string subject, string authenticationType, CancellationToken cancellationToken = default);

}
