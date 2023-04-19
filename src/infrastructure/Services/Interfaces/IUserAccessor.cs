using System.Security.Claims;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to access the current user
/// </summary>
public interface IUserAccessor
{

    /// <summary>
    /// Gets the current <see cref="ClaimsPrincipal"/>, if any
    /// </summary>
    ClaimsPrincipal? User { get; }

}
