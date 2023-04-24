namespace Hylo.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="IResourceMutator"/>s
/// </summary>
public static class IResourceMutatorExtensions
{

    /// <summary>
    /// Determines wheter or not the <see cref="IResourceMutator"/> applies to an operation performed on the specified resource kind
    /// </summary>
    /// <param name="mutator">The <see cref="IResourceMutator"/> to check</param>
    /// <param name="request">The <see cref="AdmissionReviewRequest"/> to evaluate</param>
    /// <returns>A boolean indicating wheter or not the <see cref="IResourceMutator"/> supports the specified resource kind</returns>
    public static bool AppliesTo(this IResourceMutator mutator, AdmissionReviewRequest request) => mutator.AppliesTo(request.Operation, request.Resource.Definition.Group, request.Resource.Definition.Version, request.Resource.Definition.Plural, request.Resource.Namespace);

}
