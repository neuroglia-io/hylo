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
    /// <param name="context">The context to check</param>
    /// <returns>A boolean indicating wheter or not the <see cref="IResourceMutator"/> supports the specified resource kind</returns>
    public static bool AppliesTo(this IResourceMutator mutator, AdmissionReviewContext context) => mutator.AppliesTo(context.Operation, context.Resource.Definition.Group, context.Resource.Definition.Version, context.Resource.Definition.Plural, context.Resource.Namespace);

}
