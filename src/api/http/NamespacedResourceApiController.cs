using Hylo.Api.Application.Commands.Resources.Generic;
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
    /// Patches the specified resource
    /// </summary>
    /// <param name="patch">The patch to apply</param>
    /// <param name="name">The name of the resource to patch</param>
    /// <param name="namespace">The namespace the resource to patch belongs to</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changes</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    [HttpPatch("{namespace}/{name}")]
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
    [HttpDelete("{namespace}/{name}")]
    [ProducesResponseType(typeof(IAsyncEnumerable<Resource>), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(Hylo.ProblemDetails))]
    public virtual async Task<IActionResult> DeleteResource(string name, string @namespace, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        return this.Process(await this.Mediator.Send(new DeleteResourceCommand<TResource>(name, @namespace, dryRun), cancellationToken).ConfigureAwait(false));
    }

}
