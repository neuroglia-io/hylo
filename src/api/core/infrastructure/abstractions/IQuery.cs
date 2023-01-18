using MediatR;

namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Defines the fundamentals of a query
/// </summary>
public interface IQuery
{

    /// <summary>
    /// Gets the metadata used to describe the <see cref="IQuery"/>'s context
    /// </summary>
    [JsonExtensionData]
    IDictionary<string, object>? Metadata { get; }

}

/// <summary>
/// Defines the fundamentals of a query
/// </summary>
/// <typeparam name="TResponse">The expected <see cref="IResponse"/> type</typeparam>
public interface IQuery<TResponse>
    : IQuery, IRequest<TResponse>
    where TResponse : IResponse
{



}

/// <summary>
/// Defines the fundamentals of a query
/// </summary>
/// <typeparam name="TResponse">The type of result returned by the <see cref="IQuery"/></typeparam>
/// <typeparam name="TContent">The type of content wrapped by the <see cref="IResponse{T}"/></typeparam>
public interface IQuery<TResponse, TContent>
    : IQuery<TResponse>
    where TResponse : IResponse<TContent>
{



}
