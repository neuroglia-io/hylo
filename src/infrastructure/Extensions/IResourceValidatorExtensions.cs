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
    /// <param name="context">The context to check</param>
    /// <returns>A boolean indicating wheter or not the <see cref="IResourceValidator"/> supports the specified resource kind</returns>
    public static bool AppliesTo(this IResourceValidator mutator, AdmissionReviewContext context) => mutator.AppliesTo(context.Operation, context.Resource.Definition.Group, context.Resource.Definition.Version, context.Resource.Definition.Plural, context.Resource.Namespace);

}