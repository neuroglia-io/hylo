namespace Hylo.Api.Configuration;

/// <summary>
/// Defines the fundamentals of a service used to build the <see cref="V1Resource"/> command pipeline for an Hylo API
/// </summary>
public interface IHyloApiRequestPipelineBuilder
{

    /// <summary>
    /// Gets a list containing the pipeline behaviors to use for <see cref="V1Resource"/>-related <see cref="IRequest"/>s
    /// </summary>
    List<Type> Behaviors { get; }

}