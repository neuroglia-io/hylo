namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="ICommandHandler"/>s
/// </summary>
public static class ICommandHandlerExtensions
{

    /// <summary>
    /// Returns a new <see cref="ApiResponse"/> that describes a successfull operation
    /// </summary>
    /// <typeparam name="TCommand">The type of handled <see cref="ICommand"/></typeparam>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <returns>A new <see cref="IResponse{TContent}"/></returns>
    public static IResponse Ok<TCommand>(this ICommandHandler<TCommand> handler)
        where TCommand : class, ICommand<IResponse>
    {
        return ApiResponse.Ok();
    }

    /// <summary>
    /// Returns a new <see cref="ApiResponse"/> that describes a successfull operation
    /// </summary>
    /// <typeparam name="TCommand">The type of handled <see cref="ICommand"/></typeparam>
    /// <typeparam name="TContent">The type of content to describe the successfull operation</typeparam>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="content">The content to describe the successfull operation</param>
    /// <returns>A new <see cref="IResponse{TContent}"/></returns>
    public static IResponse<TContent> Ok<TCommand, TContent>(this ICommandHandler<TCommand, TContent> handler, TContent content)
        where TCommand : class, ICommand<IResponse<TContent>>
    {
        return ApiResponse.Ok(content);
    }

    /// <summary>
    /// Returns a new <see cref="ApiResponse"/> that describes the successfull creation of the specified content
    /// </summary>
    /// <typeparam name="TCommand">The type of handled <see cref="ICommand"/></typeparam>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <returns>A new <see cref="IResponse{TContent}"/></returns>
    public static IResponse Created<TCommand>(this ICommandHandler<TCommand> handler)
        where TCommand : class, ICommand<IResponse>
    {
        return ApiResponse.Created();
    }

    /// <summary>
    /// Returns a new <see cref="ApiResponse"/> that describes the successfull creation of the specified content
    /// </summary>
    /// <typeparam name="TCommand">The type of handled <see cref="ICommand"/></typeparam>
    /// <typeparam name="TContent">The type of content to describe the successfull creation of</typeparam>
    /// <param name="handler">The extended <see cref="ICommandHandler"/></param>
    /// <param name="content">The content to describe the successfull creation of</param>
    /// <returns>A new <see cref="IResponse{TContent}"/></returns>
    public static IResponse<TContent> Created<TCommand, TContent>(this ICommandHandler<TCommand, TContent> handler, TContent content)
        where TCommand : class, ICommand<IResponse<TContent>>
    {
        return ApiResponse.Created(content);
    }

}