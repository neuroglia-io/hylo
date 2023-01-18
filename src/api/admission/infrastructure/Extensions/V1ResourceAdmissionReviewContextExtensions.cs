using Hylo.Api.Configuration;

namespace Hylo.Api.Admission.Infrastructure.Services;

/// <summary>
/// Defines extensions for <see cref="V1Resource"/>s
/// </summary>
public static class V1ResourceAdmissionReviewContextExtensions
{

    /// <summary>
    /// Converts the <see cref="ResourceAdmissionReviewContext"/> for a specific <see cref="V1Resource"/> type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="V1Resource"/> to admit</typeparam>
    /// <param name="context">The <see cref="ResourceAdmissionReviewContext"/> to convert</param>
    /// <returns>A new generic version of the context</returns>
    public static ResourceAdmissionReviewContext<TResource> OfType<TResource>(this ResourceAdmissionReviewContext context)
        where TResource : V1Resource, new()
    {
        return new(context);
    }

}

/// <summary>
/// Defines extensions for <see cref="IHyloApiBuilder"/>s
/// </summary>
public static class IHyloApiBuilderExtensions
{

    /// <summary>
    /// Adds the V1 Admission API to the Hylo API to build
    /// </summary>
    /// <param name="api">The service used to configure an Hylo API</param>
    /// <returns>The configured <see cref="IHyloApiBuilder"/></returns>
    public static IHyloApiBuilder AddAdmissionApiV1(this IHyloApiBuilder api)
    {
        foreach (var resource in V1AdmissionApiDefaults.Resources.BuiltInDefinitions.AsEnumerable())
        {
            api.Resources.RegisterResourceDefinition(resource);
        }
        return api;
    }

}