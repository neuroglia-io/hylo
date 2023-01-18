using Hylo.Api.Admission.Infrastructure.Services;
using Hylo.Api.Core.Infrastructure;
using Hylo.Api.Core.Infrastructure.Mediation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hylo.Api.Admission.Infrastructure.Mediation.PipelineBehaviors;

/// <summary>
/// Represents the <see cref="ResourcePipelineBehaviorBase"/> used to mutate resources during admission using webhooks
/// </summary>
public class ResourceAdmissionPipelineBehavior
    : ResourcePipelineBehaviorBase
{

    /// <summary>
    /// Initializes a new <see cref="ResourceAdmissionPipelineBehavior"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="resources">The service used to manage <see cref="V1Resource"/>s</param>
    /// <param name="resourceAdmissionControl">Tje service used to manage well known <see cref="V1ResourceDefinition"/>s</param>
    /// <param name="httpClient">The service used to perform webhook requests</param>
    public ResourceAdmissionPipelineBehavior(ILoggerFactory loggerFactory, IResourceRepository resources, IResourceAdmissionControl resourceAdmissionControl, HttpClient httpClient)
        : base(loggerFactory, resources)
    {
        this.ResourceAdmissionControl = resourceAdmissionControl;
        this.HttpClient = httpClient;
    }

    /// <summary>
    /// Gets the service used to perform webhook requests
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <summary>
    /// Gets the service used to control <see cref="V1Resource"/> admission
    /// </summary>
    protected IResourceAdmissionControl ResourceAdmissionControl { get; }

    /// <inheritdoc/>
    public override async Task<IResponse<object>> Handle(ResourceCommand command, RequestHandlerDelegate<IResponse<object>> next, CancellationToken cancellationToken)
    {
        var resourceReference = command.GetResourceReference();
        var resourceDefinition = await this.Resources.GetResourceDefinitionAsync(resourceReference, cancellationToken);
        if (resourceDefinition == null) return ApiResponse.ResourceDefinitionNotFound<object>(resourceReference.Group, resourceReference.Version, resourceReference.Plural);
        var context = new ResourceAdmissionReviewContext(command.Operation, resourceDefinition, command.GetResourceReference(), command.Resource);
        await this.ResourceAdmissionControl.EvaluateAsync(context, cancellationToken);
        if (!context.Succeeded) return ApiResponse.ResourceAdmissionFailed(command.Operation, command.Group, command.Version, command.Plural, context.Reviews.Where(r => r.Response != null && r.Response.Errors != null).SelectMany(r => r.Response!.Errors!));
        return await next();
    }

}
