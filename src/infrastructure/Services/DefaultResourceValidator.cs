using Hylo.Properties;
using Json.Patch;
using Json.Pointer;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the <see cref="IResourceValidator"/> used to validate the semantics of <see cref="IResource"/>s being admitted, and possibly mutate them
/// </summary>
public class DefaultResourceValidator
    : IResourceMutator
{

    /// <summary>
    /// Initializes a new <see cref="DefaultResourceValidator"/>
    /// </summary>
    /// <param name="repository">The service used to manage the application's <see cref="IResource"/>s</param>
    public DefaultResourceValidator(IRepository repository)
    {
        this.Repository = repository;
    }

    /// <summary>
    /// Gets the service used to manage the application's <see cref="IResource"/>s
    /// </summary>
    protected IRepository Repository { get; }

    /// <inheritdoc/>
    public bool AppliesTo(Operation operation, string group, string version, string plural, string? @namespace = null) => operation == Operation.Create;

    /// <inheritdoc/>
    public virtual async Task<AdmissionReviewResponse> MutateAsync(AdmissionReviewRequest context, CancellationToken cancellationToken = default)
    {
        var resourceDefinition = await this.Repository.GetDefinitionAsync(context.Resource.Definition.Group, context.Resource.Definition.Plural, cancellationToken).ConfigureAwait(false);
        var patchOperations = new List<PatchOperation>();
        if (resourceDefinition == null) return new AdmissionReviewResponse(context.Uid, false, null, ProblemDetails.ResourceDefinitionNotFound(context.Resource.Definition));
        if (resourceDefinition.Spec.Scope == ResourceScope.Cluster && !string.IsNullOrWhiteSpace(context.Resource.Namespace))
            return new AdmissionReviewResponse(context.Uid, false, null, ProblemDetails.ResourceAdmissionFailed(context.Operation, context.Resource,
                new KeyValuePair<string, string[]>(JsonPointer.Create<Resource>(r => r.Metadata.Namespace!).ToString().ToCamelCase(), new[] { StringExtensions.Format(ProblemDescriptions.ClusterResourceCannotDefineNamespace, context.Resource) })));
        else if (resourceDefinition.Spec.Scope == ResourceScope.Namespaced && string.IsNullOrWhiteSpace(context.Resource.Namespace))
            patchOperations.Add(PatchOperation.Add(JsonPointer.Create<Resource>(r => r.Metadata.Namespace!).ToCamelCase(), Serializer.Json.SerializeToNode(Namespace.DefaultNamespaceName)));
        var resourceName = context.Resource.Name;
        if (resourceName.EndsWith("-")) resourceName = $"{context.Resource.Name}{Guid.NewGuid().ToShortString().ToLowerInvariant()}";
        if (!ObjectNamingConvention.Current.IsValidResourceName(resourceName))
            return new AdmissionReviewResponse(context.Uid, false, null, ProblemDetails.ResourceAdmissionFailed(context.Operation, context.Resource,
                new KeyValuePair<string, string[]>(JsonPointer.Create<Resource>(r => r.Metadata.Name!).ToString().ToCamelCase(), new[] { StringExtensions.Format(ProblemDescriptions.InvalidResourceName, context.Resource.Name) })));
        if (!string.IsNullOrWhiteSpace(context.Resource.Namespace) && !ObjectNamingConvention.Current.IsValidResourceName(context.Resource.Namespace))
            return new AdmissionReviewResponse(context.Uid, false, null, ProblemDetails.ResourceAdmissionFailed(context.Operation, context.Resource,
                new KeyValuePair<string, string[]>(JsonPointer.Create<Resource>(r => r.Metadata.Namespace!).ToString().ToCamelCase(), new[] { StringExtensions.Format(ProblemDescriptions.InvalidResourceName, context.Resource.Namespace) })));
        var patch = patchOperations.Any() ? new Patch(PatchType.JsonPatch, new JsonPatch(patchOperations)) : null;
        return new(context.Uid, true, patch);
    }

}
