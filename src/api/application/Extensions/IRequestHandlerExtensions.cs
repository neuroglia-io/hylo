using System.Net;

namespace Hylo.Api.Application;

/// <summary>
/// Defines extensions for <see cref="IRequestHandler{TRequest}"/> instances
/// </summary>
public static class IRequestHandlerExtensions
{

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> that describes the successfull execution of a request
    /// </summary>
    /// <typeparam name="TRequest">The type of request that was successfully executed</typeparam>
    /// <param name="handler">The extended <see cref="IRequestHandler{TRequest}"/></param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse Ok<TRequest>(this IRequestHandler<TRequest> handler)
        where TRequest : class, IRequest
    {
        return new((int)HttpStatusCode.OK);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> that describes the successfull execution of a request
    /// </summary>
    /// <typeparam name="TRequest">The type of request that was successfully executed</typeparam>
    /// <typeparam name="TResult">The type of the request's result</typeparam>
    /// <param name="handler">The extended <see cref="IRequestHandler{TRequest}"/></param>
    /// <param name="result">The request's result</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<TResult> Ok<TRequest, TResult>(this IRequestHandler<TRequest, TResult> handler, TResult? result)
        where TRequest : class, IRequest<TResult>
    {
        return new((int)HttpStatusCode.OK, result);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> that describes the successfull execution of a request
    /// </summary>
    /// <typeparam name="TRequest">The type of request that was successfully executed</typeparam>
    /// <param name="handler">The extended <see cref="IRequestHandler{TRequest}"/></param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse Created<TRequest>(this IRequestHandler<TRequest> handler)
        where TRequest : class, IRequest
    {
        return new((int)HttpStatusCode.Created);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> that describes the successfull execution of a request
    /// </summary>
    /// <typeparam name="TRequest">The type of request that was successfully executed</typeparam>
    /// <typeparam name="TResult">The type of the request's result</typeparam>
    /// <param name="handler">The extended <see cref="IRequestHandler{TRequest}"/></param>
    /// <param name="result">The request's result</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<TResult> Created<TRequest, TResult>(this IRequestHandler<TRequest, TResult> handler, TResult? result)
        where TRequest : class, IRequest<TResult>
    {
        return new((int)HttpStatusCode.Created, result);
    }

}
