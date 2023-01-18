using MediatR;

namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="IQuery"/> instances of the specified type
/// </summary>
public interface IQueryHandler
{



}

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="IQuery"/> instances of the specified type
/// </summary>
/// <typeparam name="TQuery">The type of <see cref="IQuery"/>s to handle</typeparam>
/// <typeparam name="TContent">The type of content wrapped by the resulting <see cref="IResponse"/></typeparam>
public interface IQueryHandler<TQuery, TContent>
    : IQueryHandler, IRequestHandler<TQuery, IResponse<TContent>>
    where TQuery : class, IQuery<IResponse<TContent>>
{



}