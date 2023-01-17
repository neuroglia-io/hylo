using Hylo.Api.Core.Infrastructure;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;

namespace Hylo.Api.Server;

/// <summary>
/// Represents the API's <see cref="AuthorizeAttribute"/>
/// </summary>
public class ApiAuthorizeAttribute
    : AuthorizeAttribute
{

    /// <summary>
    /// Initializes a new <see cref="ApiAuthorizeAttribute"/>
    /// </summary>
    public ApiAuthorizeAttribute()
    {
        this.AuthenticationSchemes = string.Join(',', BasicAuthenticationDefaults.AuthenticationScheme, CertificateAuthenticationDefaults.AuthenticationScheme);
    }

}
