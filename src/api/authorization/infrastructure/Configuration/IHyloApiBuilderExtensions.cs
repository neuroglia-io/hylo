using Hylo.Api.Authorization.Infrastructure.Services;
using Hylo.Api.Configuration;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hylo.Api.Authorization.Infrastructure.Configuration;

/// <summary>
/// Defines extensions for <see cref="IHyloApiBuilderExtensions"/>
/// </summary>
public static class IHyloApiBuilderExtensions
{

    /// <summary>
    /// Configures Hylo to use the authorization API
    /// </summary>
    /// <param name="builder">The <see cref="IHyloApiBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IHyloApiBuilder"/></returns>
    public static IHyloApiBuilder UseAuthorizationApi(this IHyloApiBuilder builder)
    {
        builder.Services.AddSingleton<AuthenticationManager>();
        builder.Services.AddSingleton<IAuthenticationManager>(provider => provider.GetRequiredService<AuthenticationManager>());
        builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<AuthenticationManager>());

        builder.Services.AddSingleton<IdentityRoleManager>();
        builder.Services.AddSingleton<IIdentityRoleManager>(provider => provider.GetRequiredService<IdentityRoleManager>());
        builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<IdentityRoleManager>());

        builder.Services.AddSingleton<IdentityClaimsPrincipalFactory>();
        builder.Services.AddSingleton<IIdentityClaimsPrincipalFactory>(provider => provider.GetRequiredService<IdentityClaimsPrincipalFactory>());
        builder.Services.AddSingleton<IIdentityClaimsPrincipalFactory>(provider => provider.GetRequiredService<IdentityClaimsPrincipalFactory>());

        builder.Services.AddAuthentication()
            .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(BasicAuthenticationDefaults.AuthenticationScheme, options => { })
            .AddCertificate(options =>
            {
                options.Events = new()
                {
                    OnCertificateValidated = async (CertificateValidatedContext context) =>
                    {
                        var authenticationManager = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationManager>();
                        var result = await authenticationManager.AuthenticateAsync(CertificateAuthenticationDefaults.AuthenticationScheme, new CertificateAuthenticationProperties(context.ClientCertificate));
                        if (!result.Succeeded)
                        {
                            context.Fail(result.Failure!);
                            return;
                        }
                        context.Principal = result.Principal;
                        context.Success();
                    }
                };
            });
        builder.Services.AddAuthorization();

        foreach (var resource in V1AuthorizationApiDefaults.Resources.BuiltInDefinitions.AsEnumerable())
        {
            builder.Resources.RegisterResourceDefinition(resource);
        }

        return builder;
    }

}
