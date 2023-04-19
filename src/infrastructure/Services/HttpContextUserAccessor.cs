using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents an <see cref="IHttpContextAccessor"/> based <see cref="IUserAccessor"/> implementation
/// </summary>
public class HttpContextUserAccessor
    : IUserAccessor
{

    /// <summary>
    /// Initializes a new <see cref="HttpContextUserAccessor"/>
    /// </summary>
    /// <param name="httpContextAccessor">The service used to access the current <see cref="Microsoft.AspNetCore.Http.HttpContext"/></param>
    public HttpContextUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        this.HttpContext = httpContextAccessor.HttpContext;
    }

    /// <summary>
    /// Gets the current <see cref="Microsoft.AspNetCore.Http.HttpContext"/>
    /// </summary>
    protected HttpContext? HttpContext { get; }

    /// <inheritdoc/>
    public ClaimsPrincipal? User => this.HttpContext == null ? ClaimsPrincipal.Current : this.HttpContext.User;

}