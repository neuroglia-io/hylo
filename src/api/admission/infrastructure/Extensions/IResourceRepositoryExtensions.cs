namespace Hylo.Api.Admission.Infrastructure.Services;

/// <summary>
/// Defines extensions for <see cref="IResourceRepository"/> implementations
/// </summary>
public static class IResourceRepositoryExtensions
{

    /// <summary>
    /// Gets <see cref="V1MutatingWebhook"/>s that apply to the specified resource and operation
    /// </summary>
    /// <param name="resources">The <see cref="IResourceRepository"/> to query</param>
    /// <param name="operation">The operation for which to retrieve matching <see cref="V1MutatingWebhook"/>s</param>
    /// <param name="resource">An object used to reference the resource to get <see cref="V1MutatingWebhook"/>s for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing all matching <see cref="V1MutatingWebhook"/>s</returns>
    public static IAsyncEnumerable<V1MutatingWebhook> GetMutatingWebhooksFor(this IResourceRepository resources, V1ResourceReference resource, string operation, CancellationToken cancellationToken = default)
    {
        return resources
            .OfType<V1MutatingWebhook>(V1MutatingWebhook.HyloGroup, V1MutatingWebhook.HyloApiVersion, V1MutatingWebhook.HyloPluralName, cancellationToken: cancellationToken)
            .Where(wh => wh.Spec.Resources == null ? true : wh.Spec.Resources.Any(r => r.Matches(resource.Group, resource.Version, resource.Plural, operation)));
    }

    /// <summary>
    /// Gets <see cref="V1ValidatingWebhook"/>s that apply to the specified resource and operation
    /// </summary>
    /// <param name="resources">The <see cref="IResourceRepository"/> to query</param>
    /// <param name="operation">The operation for which to retrieve matching <see cref="V1ValidatingWebhook"/>s</param>
    /// <param name="resource">An object used to reference the resource to get <see cref="V1ValidatingWebhook"/>s for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing all matching <see cref="V1ValidatingWebhook"/>s</returns>
    public static IAsyncEnumerable<V1ValidatingWebhook> GetValidatingWebhooksFor(this IResourceRepository resources, V1ResourceReference resource, string operation, CancellationToken cancellationToken = default)
    {
        return resources
            .OfType<V1ValidatingWebhook>(V1ValidatingWebhook.HyloGroup, V1ValidatingWebhook.HyloApiVersion, V1ValidatingWebhook.HyloPluralName, cancellationToken: cancellationToken)
            .Where(wh => wh.Spec.Resources == null ? true : wh.Spec.Resources.Any(r => r.Matches(resource.Group, resource.Version, resource.Plural, operation)));
    }

}
