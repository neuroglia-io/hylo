using Hylo.Api.Configuration;
using Hylo.Api.Core.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.Api.Authorization.Core.Configuration;

/// <summary>
/// Defines extensions for <see cref="IHyloApiBuilderExtensions"/>
/// </summary>
public static class IHyloApiBuilderExtensions
{

    /// <summary>
    /// Configures Hylo to use the core API
    /// </summary>
    /// <param name="builder">The <see cref="IHyloApiBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IHyloApiBuilder"/></returns>
    public static IHyloApiBuilder UseCoreApi(this IHyloApiBuilder builder)
    {
        builder.Services.AddHostedService<ApiResourcesInitializer>();

        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IResourceVersionControl, ResourceVersionControl>();

        foreach (var resource in V1CoreApiDefaults.Resources.BuiltInDefinitions.AsEnumerable())
        {
            builder.Resources.RegisterResourceDefinition(resource);
        }

        return builder;
    }

}
