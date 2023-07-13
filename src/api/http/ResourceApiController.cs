﻿using Hylo.Api.Application.Commands.Resources.Generic;
using Hylo.Api.Application.Queries.Resources.Generic;
using System.Net;

namespace Hylo.Api.Http;

/// <summary>
/// Represents the base class of <see cref="ApiController"/>s used to manage <see cref="IResource"/>s
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/> to managae</typeparam>
public abstract class ResourceApiController<TResource>
    : ApiController
    where TResource : class, IResource, new()
{

    /// <inheritdoc/>
    protected ResourceApiController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Creates a new resource of the specified type
    /// </summary>
    /// <param name="resource">The resource to create</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changes</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpPost]
    [ProducesResponseType(typeof(Resource), (int)HttpStatusCode.Created)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateResource([FromBody] TResource resource, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        return this.Process(await this.Mediator.Send(new CreateResourceCommand<TResource>(resource, dryRun), cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Gets the definition of the managed resources
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpGet("definition")]
    [ProducesResponseType(typeof(ResourceDefinition), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public virtual async Task<IActionResult> GetResourceDefinition(CancellationToken cancellationToken = default)
    {
        return this.Process(await this.Mediator.Send(new GetResourceDefinitionQuery<TResource>(), cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Asynchronously enumerates through all resources that matches the specified label selector, if any. To enumerate asynchronously, consumers must read the response stream in chunks
    /// </summary>
    /// <param name="labelSelector">A comma-separated list of label selectors, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IAsyncEnumerable<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public virtual async Task<IActionResult> GetClusterResources(string? labelSelector = null, CancellationToken cancellationToken = default)
    {
        if (!this.TryParseLabelSelectors(labelSelector, out var labelSelectors)) return this.InvalidLabelSelector(labelSelector!);
        return this.Process(await this.Mediator.Send(new GetResourcesQuery<TResource>(null, labelSelectors), cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Lists matching resources
    /// </summary>
    /// <param name="labelSelector">A comma-separated list of label selectors, if any</param>
    /// <param name="maxResults">The maximum amount, if any, of results to list at once</param>
    /// <param name="continuationToken">A token, defined by a previously retrieved collection, used to continue enumerating through matches</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpGet("list")]
    [ProducesResponseType(typeof(Collection<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public virtual async Task<IActionResult> ListClusterResources(string? labelSelector = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        if (!this.TryParseLabelSelectors(labelSelector, out var labelSelectors)) return this.InvalidLabelSelector(labelSelector!);
        return this.Process(await this.Mediator.Send(new ListResourcesQuery<TResource>(null, labelSelectors, maxResults, continuationToken), cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Watches matching resources
    /// </summary>
    /// <param name="labelSelector">A comma-separated list of label selectors, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpGet("watch")]
    [ProducesResponseType(typeof(IAsyncEnumerable<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public virtual async Task<IActionResult> WatchClusterResources(string? labelSelector = null, CancellationToken cancellationToken = default)
    {
        if (!this.TryParseLabelSelectors(labelSelector, out var labelSelectors)) return this.InvalidLabelSelector(labelSelector!);
        var response = await this.Mediator.Send(new WatchResourcesQuery<TResource>(null, labelSelectors), cancellationToken).ConfigureAwait(false);
        if (!response.Status.IsSuccessStatusCode()) return this.Process(response);
        var watch = response.Content!;
        return this.Ok(watch.ToAsyncEnumerable());
    }

    /// <summary>
    /// Replaces the specified resource
    /// </summary>
    /// <param name="resource">The resource to replace</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changes</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpPut]
    [ProducesResponseType(typeof(IAsyncEnumerable<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public virtual async Task<IActionResult> ReplaceResource(TResource resource, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        return this.Process(await this.Mediator.Send(new ReplaceResourceCommand<TResource>(resource, dryRun), cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Parses the specified string into a new <see cref="List{T}"/> of <see cref="LabelSelector"/>s
    /// </summary>
    /// <param name="labelSelector">The string to parse</param>
    /// <param name="labelSelectors">A new <see cref="List{T}"/> containing the parsed <see cref="LabelSelector"/>s</param>
    /// <returns>A boolean indicating whether or not the input could be parse</returns>
    protected virtual bool TryParseLabelSelectors(string? labelSelector, out IEnumerable<LabelSelector>? labelSelectors)
    {
        labelSelectors = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(labelSelector)) labelSelectors = LabelSelector.ParseList(labelSelector);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a new <see cref="IActionResult"/> that describes an error while parsing the request's label selector
    /// </summary>
    /// <param name="labelSelector">The invalid label selector</param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    protected IActionResult InvalidLabelSelector(string labelSelector)
    {
        this.ModelState.AddModelError(nameof(labelSelector), $"The specified value '{labelSelector}' is not a valid comma-separated label selector list");
        return this.ValidationProblem("Bad Request", statusCode: (int)HttpStatusCode.BadRequest, title: "Bad Request", modelStateDictionary: this.ModelState);
    }

}
