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
    public virtual async Task<AdmissionReviewResult> ReviewAsync(AdmissionReviewRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        var context = new AdmissionReviewContext(request);
        await this.MutateAsync(context, cancellationToken).ConfigureAwait(false);
        if (context.Allowed) await this.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
    }


    /// <summary>
    /// Mutates the specified <see cref="IResource"/> upon admission
    /// </summary>
    /// <param name="context">The current <see cref="AdmissionReviewContext"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task MutateAsync(AdmissionReviewContext context, CancellationToken cancellationToken)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var mutators = this.Mutators.Where(m => m.AppliesTo(context)).ToList();

        mutators.AddRange(await this.ServiceProvider.GetRequiredService<IRepository>()
            .GetMutatingWebhooksFor(context.Request.Operation, context.Request.Resource, cancellationToken)
            .Select(wh => ActivatorUtilities.CreateInstance<WebhookResourceMutator>(this.ServiceProvider, wh))
            .ToListAsync(cancellationToken).ConfigureAwait(false));

        foreach (var mutator in mutators)
        {
            await mutator.MutateAsync(context, cancellationToken).ConfigureAwait(false);
            if (!context.Allowed) break;
        }
    }

    /// <summary>
    /// Validates the specified <see cref="IResource"/> upon admission
    /// </summary>
    /// <param name="context">The current <see cref="AdmissionReviewContext"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ValidateAsync(AdmissionReviewContext context, CancellationToken cancellationToken)
    {
        if(context == null) throw new ArgumentNullException(nameof(context));

        var validators = this.Validators.Where(m => m.AppliesTo(context)).ToList();
        
        validators.AddRange(await this.ServiceProvider.GetRequiredService<IRepository>()
            .GetMutatingWebhooksFor(context.Request.Operation, context.Request.Resource, cancellationToken)
            .Select(wh => ActivatorUtilities.CreateInstance<WebhookResourceValidator>(this.ServiceProvider, wh))
            .ToListAsync(cancellationToken));
      

        var tasks = new List<Task>(validators.Count);
        foreach (var validator in validators)
        {
            tasks.Add(validator.ValidateAsync(context, cancellationToken));
        }
        await Task.WhenAll(tasks).ConfigureAwait(false);

    }

}
