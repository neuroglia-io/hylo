using Hylo.Api.Application.Commands.Resources.Generic;
using Hylo.Api.Application.Queries.Resources.Generic;
using System.Net;

namespace Hylo.Api.Http;

/// <summary>
/// Represents a <see cref="ResourceApiController{TResource}"/> used to manage namespaced resources
/// </summary>
/// <typeparam name="TResource">The type of namespaced <see cref="IResource"/> to manage</typeparam>
public abstract class NamespacedResourceApiController<TResource>
    : ResourceApiController<TResource>
    where TResource : class, IResource, new()
{

    /// <inheritdoc/>
    protected NamespacedResourceApiController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Gets the specified namespaced resourced
    /// </summary>
    /// <param name="name">The name of the resource to get</param>
    /// <param name="namespace">The namespace the resource to get belong to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpGet("namespace/{namespace}/{name}")]
    [ProducesResponseType(typeof(IAsyncEnumerable<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public virtual async Task<IActionResult> GetNamespacedResource(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        return this.Process(await this.Mediator.Send(new GetResourceQuery<TResource>(name, @namespace), cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Asynchronously enumerates through all resources that matches the specified label selector, if any. To enumerate asynchronously, consumers must read the response stream in chunks
    /// </summary>
    /// <param name="namespace">The namespace the resources to enumerate belong to</param>
    /// <param name="labelSelector">A comma-separated list of label selectors, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpGet("namespace/{namespace}")]
    [ProducesResponseType(typeof(IAsyncEnumerable<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public virtual async Task<IActionResult> GetNamespacedResources(string @namespace, string? labelSelector = null, CancellationToken cancellationToken = default)
    {
        if (!this.TryParseLabelSelectors(labelSelector, out var labelSelectors)) return this.InvalidLabelSelector(labelSelector!);
        return this.Process(await this.Mediator.Send(new GetResourcesQuery<TResource>(@namespace, labelSelectors), cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Lists matching resources
    /// </summary>
    /// <param name="namespace">The namespace the resources to list belong to</param>
    /// <param name="labelSelector">A comma-separated list of label selectors, if any</param>
    /// <param name="maxResults">The maximum amount, if any, of results to list at once</param>
    /// <param name="continuationToken">A token, defined by a previously retrieved collection, used to continue enumerating through matches</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpGet("namespace/{namespace}/list")]
    [ProducesResponseType(typeof(Collection<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public virtual async Task<IActionResult> ListNamespacedResources(string @namespace, string? labelSelector = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        if (!this.TryParseLabelSelectors(labelSelector, out var labelSelectors)) return this.InvalidLabelSelector(labelSelector!);
        return this.Process(await this.Mediator.Send(new ListResourcesQuery<TResource>(@namespace, labelSelectors, maxResults, continuationToken), cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Watches matching resources
    /// </summary>
    /// <param name="namespace">The namespace the resources to watch belong to</param>
    /// <param name="labelSelector">A comma-separated list of label selectors, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpGet("namespace/{namespace}/watch")]
    [ProducesResponseType(typeof(IAsyncEnumerable<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public virtual async Task<IActionResult> WatchNamespacedResources(string @namespace, string? labelSelector = null, CancellationToken cancellationToken = default)
    {
        if (!this.TryParseLabelSelectors(labelSelector, out var labelSelectors)) return this.InvalidLabelSelector(labelSelector!);
        var response = await this.Mediator.Send(new WatchResourcesQuery<TResource>(@namespace, labelSelectors), cancellationToken).ConfigureAwait(false);
        if (!response.Status.IsSuccessStatusCode()) return this.Process(response);
        var watch = response.Content!;
        return this.Ok(watch.ToAsyncEnumerable());
    }

    /// <summary>
    /// Patches the specified resource
    /// </summary>
    /// <param name="patch">The patch to apply</param>
    /// <param name="name">The name of the resource to patch</param>
    /// <param name="namespace">The namespace the resource to patch belongs to</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changes</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpPatch("namespace/{namespace}/{name}")]
    [ProducesResponseType(typeof(IAsyncEnumerable<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public virtual async Task<IActionResult> PatchResource(string name, string @namespace, [FromBody] Patch patch, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        return this.Process(await this.Mediator.Send(new PatchResourceCommand<TResource>(name, @namespace, patch, dryRun), cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Deletes the specified resource
    /// </summary>
    /// <param name="name">The name of the resource to delete</param>
    /// <param name="namespace">The namespace the resource to patch belongs to</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changes</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpDelete("namespace/{namespace}/{name}")]
    [ProducesResponseType(typeof(IAsyncEnumerable<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(Hylo.ProblemDetails))]
    public virtual async Task<IActionResult> DeleteResource(string name, string @namespace, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        return this.Process(await this.Mediator.Send(new DeleteResourceCommand<TResource>(name, @namespace, dryRun), cancellationToken).ConfigureAwait(false));
    }

}
