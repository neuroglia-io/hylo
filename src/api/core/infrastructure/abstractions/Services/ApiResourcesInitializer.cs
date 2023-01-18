using Hylo.Api.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IApiResourcesInitializer"/> interface
/// </summary>
public class ApiResourcesInitializer
    : BackgroundService, IApiResourcesInitializer
{

    /// <summary>
    /// Initializes a new <see cref="ApiResourcesInitializer"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="options">The service used to access the current <see cref="HyloApiResourceInitializerOptions"/></param>
    public ApiResourcesInitializer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<HyloApiResourceInitializerOptions> options)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Options = options.Value;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the current <see cref="HyloApiResourceInitializerOptions"/>
    /// </summary>
    protected HyloApiResourceInitializerOptions Options { get; }

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return this.InitializeAsync(stoppingToken);
    }

    /// <inheritdoc/>
    public virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using var scope = this.ServiceProvider.CreateScope();
        var resources = scope.ServiceProvider.GetRequiredService<IResourceRepository>();
        if(await resources.GetResourceDefinitionAsync(V1Namespace.HyloGroup, V1Namespace.HyloVersion, V1Namespace.HyloPluralName, cancellationToken) != null)
        {
            this.Logger.LogDebug("Resource initialization skipped: already initialized");
            return;
        }
        if(this.Options.ResourceDefinitions == null)
        {
            this.Logger.LogDebug("Resource initialization skipped: no well-known resources registered");
            return;
        }
        this.Logger.LogDebug("Initializing API resources...");
        foreach (var resource in this.Options.ResourceDefinitions)
        {
            await resources.AddResourceAsync(V1ResourceDefinition.HyloGroup, V1ResourceDefinition.HyloVersion, V1ResourceDefinition.HyloPluralName, resource, cancellationToken);
        }
        await resources.SaveChangesAsync(cancellationToken);
        this.Logger.LogDebug("API resources have been successfully initialized");
    }

}
