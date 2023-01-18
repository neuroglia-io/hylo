using MediatR;

namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s of the specified type
/// </summary>
public interface ICommandHandler
{



}

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s of the specified type
/// </summary>
/// <typeparam name="TCommand">The type of <see cref="ICommand"/>s to handle</typeparam>
public interface ICommandHandler<TCommand>
    : ICommandHandler, IRequestHandler<TCommand, IResponse>
    where TCommand : class, ICommand<IResponse>
{



}

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s of the specified type
/// </summary>
/// <typeparam name="TCommand">The type of <see cref="ICommand"/>s to handle</typeparam>
/// <typeparam name="TContent">The type of content wrapped by the resulting <see cref="IResponse"/></typeparam>
public interface ICommandHandler<TCommand, TContent>
    : ICommandHandler, IRequestHandler<TCommand, IResponse<TContent>>
    where TCommand : class, ICommand<IResponse<TContent>>
{



}
