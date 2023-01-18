using Hylo.Api.Admission.Infrastructure.Services;
using Hylo.Api.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.Api.Admission.Infrastructure.Configuration;

/// <summary>
/// Defines extensions for <see cref="IHyloApiBuilder"/>s
/// </summary>
public static class IHyloApiBuilderExtensions
{

    /// <summary>
    /// Configures Hylo to use the admission API
    /// </summary>
    /// <param name="builder">The <see cref="IHyloApiBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IHyloApiBuilder"/></returns>
    public static IHyloApiBuilder UseAdmissionApi(this IHyloApiBuilder builder)
    {
        builder.Services.AddScoped<IResourceAdmissionControl, ResourceAdmissionControl>();

        foreach(var resource in V1AdmissionApiDefaults.Resources.BuiltInDefinitions.AsEnumerable())
        {
            builder.Resources.RegisterResourceDefinition(resource);
        }

        return builder;
    }

}
