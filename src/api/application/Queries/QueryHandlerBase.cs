namespace Hylo.Api.Queries;

/// <summary>
/// Represents the base class for all <see cref="ICommandHandler"/>s
/// </summary>
public abstract class QueryHandlerBase
{

    /// <summary>
    /// Initializes a new <see cref="QueryHandlerBase"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    protected QueryHandlerBase(ILoggerFactory loggerFactory) 
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

}
