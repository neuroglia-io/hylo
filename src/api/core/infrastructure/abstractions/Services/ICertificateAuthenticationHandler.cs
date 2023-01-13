using Microsoft.AspNetCore.Authentication.Certificate;
using System.Security.Cryptography.X509Certificates;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="X509Certificate2"/>-based authentication
/// </summary>
public interface ICertificateAuthenticationHandler
{

    /// <summary>
    /// Handles the specified <see cref="X509Certificate2"/>-based authentication attempt
    /// </summary>
    /// <param name="context">The current <see cref="CertificateValidatedContext"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task HandleAsync(CertificateValidatedContext context, CancellationToken cancellationToken = default);

}