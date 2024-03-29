﻿namespace Hylo.Api.Application;

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="IRequest"/>s
/// </summary>
/// <typeparam name="TRequest">The type of <see cref="IRequest"/> to handle</typeparam>
public interface IRequestHandler<TRequest>
    : MediatR.IRequestHandler<TRequest, ApiResponse>
    where TRequest : IRequest
{



}

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="IRequest"/>s
/// </summary>
/// <typeparam name="TRequest">The type of <see cref="IRequest"/> to handle</typeparam>
/// <typeparam name="TResult">The expected type of result</typeparam>
public interface IRequestHandler<TRequest, TResult>
    : MediatR.IRequestHandler<TRequest, ApiResponse<TResult>>
    where TRequest : IRequest<TResult>
{



}
