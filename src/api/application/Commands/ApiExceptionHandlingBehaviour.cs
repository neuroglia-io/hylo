using MediatR;

namespace Hylo.Api.Commands;

/// <summary>
/// Represents the <see cref="IPipelineBehavior{TRequest, TResponse}"/> used to handle <see cref="ApiException"/>s
/// </summary>
/// <typeparam name="TRequest">The type of <see cref="IRequest"/> to handle</typeparam>
/// <typeparam name="TResponse">The type of <see cref="IResponse"/> to handle</typeparam>
public class ApiExceptionHandlingBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
    where TResponse : IResponse
{

    /// <inheritdoc/>
    public virtual async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch(ApiException ex)
        {
            var responseType = typeof(TResponse);
            var contentType = responseType.IsGenericType ? responseType.GetGenericArguments()[0] : null;
            if (responseType.IsInterface) responseType = contentType == null ? typeof(ApiResponse) : typeof(ApiResponse<>).MakeGenericType(contentType);
            var response = (TResponse)Activator.CreateInstance(responseType)!;
            response.Status = ex.Status;
            response.Title = ex.Title;
            response.Detail = ex.Detail;
            response.Errors = ex.Errors;
            return response;
        }
    }

}
