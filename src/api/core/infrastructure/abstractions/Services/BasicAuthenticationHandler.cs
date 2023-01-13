using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Represents the <see cref="AuthenticationHandler"/> used to handle a <see cref="BasicAuthenticationDefaults.Scheme"/> authentication scheme
/// </summary>
public class BasicAuthenticationHandler
    : AuthenticationHandler<BasicAuthenticationOptions>
{

    /// <inheritdoc/>
    public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
        : base(options, logger, encoder, clock)
    {

    }

    /// <inheritdoc/>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = this.Request.Headers.Authorization.ToString();
        if (authorizationHeader == null || !authorizationHeader.StartsWith(BasicAuthenticationDefaults.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            this.Response.StatusCode = 401;
            this.Response.Headers.WWWAuthenticate = $"{BasicAuthenticationDefaults.Scheme} realm=\"api.hylo.cloud\"";
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
        var token = authorizationHeader.Substring(BasicAuthenticationDefaults.Scheme.Length).Trim();
        var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var segments = credentials.Split(':');
        
    }

}
