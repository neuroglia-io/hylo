using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;

namespace Hylo.Api.Authorization.Infrastructure.Services;

/// <summary>
/// Represents the <see cref="AuthenticationHandler"/> used to handle a <see cref="BasicAuthenticationDefaults.AuthenticationScheme"/> authentication scheme
/// </summary>
public class BasicAuthenticationHandler
    : AuthenticationHandler<BasicAuthenticationOptions>
{

    /// <inheritdoc/>
    public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAuthenticationManager identityManager) 
        : base(options, logger, encoder, clock)
    {
        this.IdentityManager = identityManager;
    }

    /// <summary>
    /// Gets the service used to manage identities
    /// </summary>
    protected IAuthenticationManager IdentityManager { get; }

    /// <inheritdoc/>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = this.Request.Headers.Authorization.ToString();
        if (authorizationHeader == null || !authorizationHeader.StartsWith(BasicAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
        {
            this.Response.StatusCode = 401;
            this.Response.Headers.WWWAuthenticate = $"{BasicAuthenticationDefaults.AuthenticationScheme} realm=\"api.hylo.cloud\"";
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
        var encoded = authorizationHeader[BasicAuthenticationDefaults.AuthenticationScheme.Length..].Trim();
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        var segments = decoded.Split(':');
        var authenticationProperties = new BasicAuthenticationProperties(segments.First(), string.Join(':', segments[1..]));
        return await this.IdentityManager.AuthenticateAsync(this.Scheme.Name, authenticationProperties);
    }

}