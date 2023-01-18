using MediatR;

namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Defines the fundamentals of a CQRS command
/// </summary>
public interface ICommand
{

    /// <summary>
    /// Gets the metadata used to describe the <see cref="ICommand"/>'s context
    /// </summary>
    IDictionary<string, object>? Metadata { get; }

}

/// <summary>
/// Defines the fundamentals of a CQRS command 
/// </summary>
/// <typeparam name="TResponse">The expected <see cref="IResponse"/> type</typeparam>
public interface ICommand<TResponse>
    : ICommand, IRequest<TResponse>
    where TResponse : IResponse
{



}

/// <summary>
/// Defines the fundamentals of a command
/// </summary>
/// <typeparam name="TResponse">The expected <see cref="IResponse"/> type</typeparam>
/// <typeparam name="TContent">The type of content wrapped by the <see cref="IResponse"/></typeparam>
public interface ICommand<TResponse, TContent>
    : ICommand<TResponse>
    where TResponse : IResponse<TContent>
{




}
