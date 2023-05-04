namespace Hylo.Api.Http;

/// <summary>
/// Represents the base class for all <see cref="ApiController"/>s
/// </summary>
public abstract class ApiController
    : ControllerBase
{

    /// <summary>
    /// Initializes a new <see cref="ApiController"/>
    /// </summary>
    /// <param name="mediator">The service used to mediate calls</param>
    protected ApiController(IMediator mediator)
    {
        this.Mediator = mediator;
    }

    /// <summary>
    /// Gets the service used to mediate calls
    /// </summary>
    protected IMediator Mediator { get; }

    /// <summary>
    /// Processes the specified <see cref="ApiResponse"/>
    /// </summary>
    /// <param name="response">The <see cref="ApiResponse"/> to process</param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    protected virtual IActionResult Process(ApiResponse response)
    {
        if (response.Status.IsSuccessStatusCode()) return new ObjectResult(response.Content) { StatusCode = response.Status };
        return new ObjectResult(response) { StatusCode = response.Status };
    }

    /// <summary>
    /// Processes the specified <see cref="ApiResponse"/>
    /// </summary>
    /// <typeparam name="TContent">The expected type of result</typeparam>
    /// <param name="response">The <see cref="ApiResponse"/> to process</param>
    /// <returns>A new <see cref="IActionResult"/></returns>
    protected virtual IActionResult Process<TContent>(ApiResponse<TContent> response)
    {
        if (response.Status.IsSuccessStatusCode()) return new ObjectResult(response.Content) { StatusCode = response.Status };
        return new ObjectResult(response) { StatusCode = response.Status };
    }

}
