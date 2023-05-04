using FluentValidation;
using System.Net;

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
    public AdmissionControl(IServiceProvider serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <inheritdoc/>
    public virtual async Task<AdmissionReviewResponse> ReviewAsync(AdmissionReviewRequest request, CancellationToken cancellationToken = default)
    {
        var originalResource = request.UpdatedState;
        if (request == null) throw new ArgumentNullException(nameof(request));
        var result = await this.MutateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) return result;
        result = await this.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) return result;
        Patch? patch = null;
        if(originalResource != null)
        {
            patch = new(PatchType.JsonPatch, JsonPatchHelper.CreateJsonPatchFromDiff(originalResource, request.UpdatedState));
        }
        return new(request.Uid, true, patch);
    }

    /// <summary>
    /// Mutates the specified <see cref="IResource"/> upon admission
    /// </summary>
    /// <param name="request">The <see cref="AdmissionReviewRequest"/> to evaluate</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="AdmissionReviewResponse"/> that describes the result of the operation</returns>
    protected virtual async Task<AdmissionReviewResponse> MutateAsync(AdmissionReviewRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (request.Operation != Operation.Create && request.Operation != Operation.Replace && request.Operation != Operation.Patch) return new AdmissionReviewResponse(request.Uid, true);

        var mutators = this.ServiceProvider.GetServices<IResourceMutator>().Where(m => m.AppliesTo(request)).ToList();

        try
        {
            mutators.AddRange(await this.ServiceProvider.GetRequiredService<IRepository>()
                .GetMutatingWebhooksFor(request.Operation, request.Resource, cancellationToken)
                .Select(wh => ActivatorUtilities.CreateInstance<WebhookResourceMutator>(this.ServiceProvider, wh))
                .ToListAsync(cancellationToken).ConfigureAwait(false));
        }
        catch
        {
            //todo: log
        }


        foreach (var mutator in mutators)
        {
            var result = await mutator.MutateAsync(request, cancellationToken).ConfigureAwait(false);
            if (!result.Allowed) return result;
            if (result.Patch != null) request.UpdatedState = result.Patch.ApplyTo(request.UpdatedState);
        }

        return new(request.Uid, true, request.GetDiffPatch());
    }

    /// <summary>
    /// Validates the specified <see cref="IResource"/> upon admission
    /// </summary>
    /// <param name="request">The <see cref="AdmissionReviewRequest"/> to evaluate</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="AdmissionReviewResponse"/> that describes the result of the operation</returns>
    protected virtual async Task<AdmissionReviewResponse> ValidateAsync(AdmissionReviewRequest request, CancellationToken cancellationToken)
    {
        if(request == null) throw new ArgumentNullException(nameof(request));

        var validators = this.ServiceProvider.GetServices<IResourceValidator>().Where(m => m.AppliesTo(request)).ToList();
        try
        { 
            validators.AddRange(await this.ServiceProvider.GetRequiredService<IRepository>()
                .GetMutatingWebhooksFor(request.Operation, request.Resource, cancellationToken)
                .Select(wh => ActivatorUtilities.CreateInstance<WebhookResourceValidator>(this.ServiceProvider, wh))
                .ToListAsync(cancellationToken));
        }
        catch
        {
            //todo: log
        }

        var tasks = new List<Task<AdmissionReviewResponse>>(validators.Count);
        foreach (var validator in validators)
        {
            tasks.Add(validator.ValidateAsync(request, cancellationToken));
        }
        await Task.WhenAll(tasks).ConfigureAwait(false);

        var results = tasks.Select(t => t.Result);
        if (results.All(t => t.Allowed)) return new(request.Uid, true);
        else return new(request.Uid, false, null, new ProblemDetails(ProblemTypes.Resources.AdmissionFailed, Properties.ProblemTitles.AdmissionFailed, (int)HttpStatusCode.BadRequest, errors: results.Where(r => !r.Allowed && r.Problem != null && r.Problem.Errors != null).SelectMany(r => r.Problem!.Errors!)));
    }

}
