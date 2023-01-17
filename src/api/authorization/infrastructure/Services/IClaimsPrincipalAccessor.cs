using System.Security.Claims;

namespace Hylo.Api.Authorization.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to access the current <see cref="ClaimsPrincipal"/>
/// </summary>
public interface IClaimsPrincipalAccessor
{

    /// <summary>
    /// Gets the current <see cref="ClaimsPrincipal"/>, if any
    /// </summary>
    ClaimsPrincipal? User { get; }

}
