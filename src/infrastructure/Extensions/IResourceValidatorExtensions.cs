namespace Hylo.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="IResourceValidator"/>s
/// </summary>
public static class IResourceValidatorExtensions
{

    /// <summary>
    /// Determines wheter or not the <see cref="IResourceValidator"/> applies to an operation performed on the specified resource kind
    /// </summary>
    /// <param name="mutator">The <see cref="IResourceValidator"/> to check</param>
    /// <param name="request">The <see cref="AdmissionReviewRequest"/> to evaluate</param>
    /// <returns>A boolean indicating wheter or not the <see cref="IResourceValidator"/> supports the specified resource kind</returns>
    public static bool AppliesTo(this IResourceValidator mutator, AdmissionReviewRequest request) => mutator.AppliesTo(request.Operation, request.Resource.Definition.Group, request.Resource.Definition.Version, request.Resource.Definition.Plural, request.Resource.Namespace);

}