using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Hylo.Api.Authorization.Infrastructure.Services;

/// <summary>
/// Defines the default, <see cref="IHttpContextAccessor"/>-based implementation of the <see cref="IClaimsPrincipalAccessor"/> interface
/// </summary>
public class HttpContextClaimsPrincipalAccessor
    : IClaimsPrincipalAccessor
{

    /// <summary>
    /// Initializes a new <see cref="HttpContextClaimsPrincipalAccessor"/>
    /// </summary>
    /// <param name="httpContextAccessor">The service used to access the current <see cref="HttpContext"/></param>
    public HttpContextClaimsPrincipalAccessor(IHttpContextAccessor httpContextAccessor)
    {
        this.HttpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the service used to access the current <see cref="HttpContext"/>
    /// </summary>
    protected IHttpContextAccessor HttpContextAccessor { get; }

    /// <inheritdoc/>
    public virtual ClaimsPrincipal? User => this.HttpContextAccessor.HttpContext?.User;

}
