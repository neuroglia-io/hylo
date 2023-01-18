namespace Hylo.Api;

/// <summary>
/// Represents the base class for all <see cref="ICommandHandler"/>s
/// </summary>
public abstract class RequestHandlerBase
{

    /// <summary>
    /// Initializes a new <see cref="RequestHandlerBase"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    protected RequestHandlerBase(ILoggerFactory loggerFactory) 
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

}
