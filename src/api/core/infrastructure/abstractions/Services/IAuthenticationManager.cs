using Microsoft.AspNetCore.Authentication;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to authenticate users
/// </summary>
public interface IAuthenticationManager
{

    /// <summary>
    /// Performs the specified authentication attempt
    /// </summary>
    /// <param name="authenticationScheme">The scheme of the authentication to perform</param>
    /// <param name="authenticationProperties">The authentication properties</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="AuthenticationProperties"/> that describes the result of the authentication attempt</returns>
    Task<AuthenticateResult> AuthenticateAsync(string authenticationScheme, object authenticationProperties, CancellationToken cancellationToken = default);

}
