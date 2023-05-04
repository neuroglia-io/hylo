namespace Hylo.Api.Application;

/// <summary>
/// Defines the fundamentals of an application request
/// </summary>
public interface IRequest
    : MediatR.IRequest<ApiResponse>
{



}

/// <summary>
/// Defines the fundamentals of an application request
/// </summary>
/// <typeparam name="TResult">The expected type of result</typeparam>
public interface IRequest<TResult>
    : MediatR.IRequest<ApiResponse<TResult>>
{



}