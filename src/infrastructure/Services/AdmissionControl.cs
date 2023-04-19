using FluentValidation;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IAdmissionControl"/> interface
/// </summary>
public class AdmissionControl
    : IAdmissionControl
{

    /// <summary>
    /// Initializes a new <see cref="AdmissionControl"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="userInfoProvider">The service used to provide information about users</param>
    /// <param name="mutators">An <see cref="IEnumerable{T}"/> containing the services used to mutate resources</param>
    /// <param name="validators">An <see cref="IEnumerable{T}"/> containing the services used to validate resources</param>
    public AdmissionControl(IServiceProvider serviceProvider, IUserInfoProvider userInfoProvider, IEnumerable<IResourceMutator> mutators, IEnumerable<IResourceValidator> validators)
    {
        this.ServiceProvider = serviceProvider;
        this.UserInfoProvider = userInfoProvider;
        this.Mutators = mutators;
        this.Validators = validators;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets a service used to provide information about users
    /// </summary>
    protected IUserInfoProvider UserInfoProvider { get; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the services used to mutate resources
    /// </summary>
    protected IEnumerable<IResourceMutator> Mutators { get; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the services used to validate resources
    /// </summary>
    protected IEnumerable<IResourceValidator> Validators { get; }

    /// <inheritdoc/>
    public virtual async Task ReviewAsync(ResourceAdmissionReviewContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        await this.MutateAsync(context, cancellationToken).ConfigureAwait(false);
        if (context.Allowed) await this.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<ResourceAdmissionReviewResult> ReviewAsync(ResourceOperation operation, IResourceDefinition resourceDefinition, IResourceReference resource, string? subResource = null, IResource? updatedState = null, IResource? originalState = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        var context = new ResourceAdmissionReviewContext(Guid.NewGuid().ToString(), operation, resourceDefinition, resource, subResource, updatedState, originalState, this.UserInfoProvider.GetCurrentUser(), dryRun);

        await this.MutateAsync(context, cancellationToken).ConfigureAwait(false);

        if (context.Allowed) await this.ValidateAsync(context, cancellationToken).ConfigureAwait(false);

        if (context.Allowed) return new(operation == ResourceOperation.Delete ? context.OriginalState! : context.UpdatedState!);
        else return new(context.Reviews.Where(r => r.Response != null && r.Response.Problem != null).SelectMany(r => r.Response!.Problem!.Errors!).ToArray());
    }

    /// <summary>
    /// Mutates the specified <see cref="IResource"/> upon admission
    /// </summary>
    /// <param name="context">The current <see cref="ResourceAdmissionReviewContext"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task MutateAsync(ResourceAdmissionReviewContext context, CancellationToken cancellationToken)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var mutators = this.Mutators.Where(m => m.AppliesTo(context)).ToList();
        try
        {
            mutators.AddRange(await this.ServiceProvider.GetRequiredService<IResourceRepository>().GetAllMutatingWebhooksFor(context.Operation, context.Resource, cancellationToken)
                .Select(wh => ActivatorUtilities.CreateInstance<WebhookResourceMutator>(this.ServiceProvider, wh))
                .ToListAsync(cancellationToken).ConfigureAwait(false));
        }
        catch { }

        foreach (var mutator in mutators)
        {
            await mutator.MutateAsync(context, cancellationToken).ConfigureAwait(false);
            if (!context.Allowed) break;
        }
    }

    /// <summary>
    /// Validates the specified <see cref="IResource"/> upon admission
    /// </summary>
    /// <param name="context">The current <see cref="ResourceAdmissionReviewContext"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ValidateAsync(ResourceAdmissionReviewContext context, CancellationToken cancellationToken)
    {
        if(context == null) throw new ArgumentNullException(nameof(context));

        var validators = this.Validators.Where(m => m.AppliesTo(context)).ToList();
        try
        {
            validators.AddRange(await this.ServiceProvider.GetRequiredService<IResourceRepository>().GetAllMutatingWebhooksFor(context.Operation, context.Resource, cancellationToken)
            .Select(wh => ActivatorUtilities.CreateInstance<WebhookResourceValidator>(this.ServiceProvider, wh))
            .ToListAsync(cancellationToken));
        }
        catch { }

        var tasks = new List<Task>(validators.Count);
        foreach (var validator in validators)
        {
            tasks.Add(validator.ValidateAsync(context, cancellationToken));
        }
        await Task.WhenAll(tasks).ConfigureAwait(false);

    }

}
