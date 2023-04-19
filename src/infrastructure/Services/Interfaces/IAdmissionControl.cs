namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to determine whether or not to admit operations on <see cref="Resource"/>s
/// </summary>
public interface IAdmissionControl
{

    /// <summary>
    /// Reviews the specified resource operation for admission
    /// </summary>
    /// <param name="operation">The operation to perform on the resource</param>
    /// <param name="resourceDefinition">The definition of the resource to operate on</param>
    /// <param name="resource">A reference to the resource to operate on</param>
    /// <param name="subResource">A reference of the sub resource to operate on, if any</param>
    /// <param name="updatedState">The updated state of the resource to operate on. Ignored when operation is 'delete'</param>
    /// <param name="originalState">The current state of the resource to operate on. Ignored when operation is 'create'</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changed induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ResourceAdmissionReviewResult"/> that describes the admission review result</returns>
    Task<ResourceAdmissionReviewResult> ReviewAsync(ResourceOperation operation, IResourceDefinition resourceDefinition, IResourceReference resource, string? subResource = null, IResource? updatedState = null, IResource? originalState = null, bool dryRun = false, CancellationToken cancellationToken = default);

}