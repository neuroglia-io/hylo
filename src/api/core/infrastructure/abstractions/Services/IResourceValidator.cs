namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to validate <see cref="V1Resource"/>s
/// </summary>
public interface IResourceValidator
{

    /// <summary>
    /// Validates the specified <see cref="V1Resource"/>
    /// </summary>
    /// <param name="resource">The <see cref="V1Resource"/> to validate</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="V1ResourceAdmissionEvaluationResult"/> that describes the <see cref="V1Resource"/>'s validation result</returns>
    Task<V1ResourceAdmissionEvaluationResult> ValidateAsync(V1Resource resource, CancellationToken cancellationToken = default);

}

/// <summary>
/// Defines the fundamentals of a service used to validate <see cref="V1Resource"/>s
/// </summary>
/// <typeparam name="TResource">The type of <see cref="V1Resource"/> to validate</typeparam>
public interface IResourceValidator<TResource>
    : IResourceValidator
    where TResource : V1Resource
{

    /// <summary>
    /// Validates the specified <see cref="V1Resource"/>
    /// </summary>
    /// <param name="resource">The <see cref="V1Resource"/> to validate</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="V1ResourceAdmissionEvaluationResult"/> that describes the <see cref="V1Resource"/>'s validation result</returns>
    Task<V1ResourceAdmissionEvaluationResult> ValidateAsync(TResource resource, CancellationToken cancellationToken = default);

}