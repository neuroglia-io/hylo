namespace Hylo.Api.Application;

/// <summary>
/// Represents the base class for all <see cref="IResource"/>-related request handlers
/// </summary>
public abstract class ResourceRequestHandlerBase
{

    /// <summary>
    /// Initializes a new <see cref="ResourceRequestHandlerBase"/>
    /// </summary>
    /// <param name="repository">The service used to manage the application's resources</param>
    public ResourceRequestHandlerBase(IRepository repository)
    {
        this.Repository = repository;
    }

    /// <summary>
    /// Gets the service used to manage the application's resources
    /// </summary>
    protected IRepository Repository { get; }

}
