using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hylo.Api;

/// <summary>
/// Represents the base class of all Hylo API MVC controllers
/// </summary>
public abstract class ApiEndpoint
    : ControllerBase
{

    /// <summary>
    /// Initializes a new <see cref="ApiEndpoint"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="mediator">The service used to mediate calls</param>
    protected ApiEndpoint(ILoggerFactory loggerFactory, IMediator mediator)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Mediator = mediator;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to mediate calls
    /// </summary>
    protected IMediator Mediator { get; }

    /// <summary>
    /// Processes the specified <see cref="IResponse"/> and creates a new <see cref="IActionResult"/> to describe it
    /// </summary>
    /// <typeparam name="TResponse">The type of <see cref="IResponse"/> to process</typeparam>
    /// <typeparam name="TResult">The type of content wrapped by the <see cref="IResponse"/> to process</typeparam>
    /// <param name="response">The response to process</param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    protected IActionResult Process<TResponse>(TResponse response)
        where TResponse : IResponse
    {
        if(response == null) throw new ArgumentNullException(nameof(response));
        switch (response.Status)
        {
            case >= 200 and <= 299:
                return this.StatusCode(response.Status, response.Content);
            default:
                return this.StatusCode(response.Status, response);
        }
    }

}
