using Hylo.Api.Core.Infrastructure;

namespace Hylo.Api;

/// <summary>
/// Defines extensions for <see cref="IQueryHandler"/>s
/// </summary>
public static class IQueryHandlerExtensions
{

    /// <summary>
    /// Returns a new <see cref="ApiResponse"/> that describes a successfull operation
    /// </summary>
    /// <typeparam name="TQuery">The type of handled <see cref="IQuery"/></typeparam>
    /// <typeparam name="TContent">The type of content to describe the successfull operation</typeparam>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <param name="content">The content to describe the successfull operation</param>
    /// <returns>A new <see cref="IResponse{TContent}"/></returns>
    public static IResponse<TContent> Ok<TQuery, TContent>(this IQueryHandler<TQuery, TContent> handler, TContent content)
        where TQuery : class, IQuery<IResponse<TContent>>
    {
        return ApiResponse.Ok(content);
    }

    /// <summary>
    /// Returns a new <see cref="ApiResponse"/> that describes the successfull creation of the specified content
    /// </summary>
    /// <typeparam name="TQuery">The type of handled <see cref="IQuery"/></typeparam>
    /// <typeparam name="TContent">The type of content to describe the successfull creation of</typeparam>
    /// <param name="handler">The extended <see cref="IQueryHandler"/></param>
    /// <param name="content">The content to describe the successfull creation of</param>
    /// <returns>A new <see cref="IResponse{TContent}"/></returns>
    public static IResponse<TContent> Created<TQuery, TContent>(this IQueryHandler<TQuery, TContent> handler, TContent content)
        where TQuery : class, IQuery<IResponse<TContent>>
    {
        return ApiResponse.Created(content);
    }

}
