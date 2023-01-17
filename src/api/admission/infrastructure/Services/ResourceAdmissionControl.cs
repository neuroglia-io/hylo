using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hylo.Api.Admission.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IResourceAdmissionControl"/> interface
/// </summary>
public class ResourceAdmissionControl
    : IResourceAdmissionControl
{

    /// <summary>
    /// Initializes a new <see cref="ResourceAdmissionControl"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="resources">The service used to manage <see cref="V1Resource"/>s</param>
    /// <param name="resourceMutators">An <see cref="IEnumerable{T}"/> containing the services used to mutate <see cref="V1Resource"/>s</param>
    /// <param name="resourceValidators">An <see cref="IEnumerable{T}"/> containing the services used to validate <see cref="V1Resource"/>s</param>
    public ResourceAdmissionControl(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IResourceRepository resources, IEnumerable<IResourceMutator> resourceMutators, IEnumerable<IResourceValidator> resourceValidators)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Resources = resources;
        this.ResourceMutators = resourceMutators;
        this.ResourceValidators = resourceValidators;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to manage <see cref="V1Resource"/>s
    /// </summary>
    protected IResourceRepository Resources { get; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the services used to mutate <see cref="V1Resource"/>s
    /// </summary>
    protected IEnumerable<IResourceMutator> ResourceMutators { get; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the services used to validate <see cref="V1Resource"/>s
    /// </summary>
    protected IEnumerable<IResourceValidator> ResourceValidators { get; }

    /// <inheritdoc/>
    public virtual async Task EvaluateAsync(V1ResourceAdmissionReviewContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        await this.MutateAsync(context, cancellationToken).ConfigureAwait(false);
        if(context.Succeeded) await this.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Mutates the specifed resource
    /// </summary>
    /// <param name="context">The <see cref="V1ResourceAdmissionReviewContext"/> that describes the resource to mutate</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task MutateAsync(V1ResourceAdmissionReviewContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        var mutators = this.ResourceMutators.Where(m => m.SupportsResourceType(context.ResourceDefinition)).ToList();
        mutators.AddRange(await this.Resources.GetMutatingWebhooksFor(context.ResourceReference, context.Operation, cancellationToken)
            .Select(wh => ActivatorUtilities.CreateInstance<V1WebhookResourceMutator>(this.ServiceProvider, wh))
            .ToListAsync(cancellationToken));
        foreach (var mutator in mutators.OrderBy(m => m.Priority))
        {
            await mutator.MutateAsync(context, cancellationToken).ConfigureAwait(false);
            if (!context.Succeeded) break;
        }
    }

    protected virtual async Task ValidateAsync(V1ResourceAdmissionReviewContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        var validators = this.ResourceValidators.Where(m => m.SupportsResourceType(context.ResourceDefinition)).ToList();
        validators.AddRange(await this.Resources.GetValidatingWebhooksFor(context.ResourceReference, context.Operation, cancellationToken)
            .Select(wh => ActivatorUtilities.CreateInstance<V1WebhookResourceValidator>(this.ServiceProvider, wh))
            .ToListAsync(cancellationToken));
        var tasks = new List<Task>(validators.Count);
        foreach (var validator in validators)
        {
            tasks.Add(validator.ValidateAsync(context, cancellationToken));
        }
        await Task.WhenAll(tasks);
    }

}
