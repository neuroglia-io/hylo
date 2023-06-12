using Hylo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hylo.Providers.FileSystem.Services;

/// <summary>
/// Represents the <see cref="IPluginBootstrapper"/> implementation used to configure the <see cref="FileSystemDatabaseProvider"/> plugin
/// </summary>
public class FileSystemDatabaseProviderPluginBoostrapper
    : IPluginBootstrapper
{

    /// <summary>
    /// Initializes a new <see cref="FileSystemDatabaseProviderPluginBoostrapper"/>
    /// </summary>
    /// <param name="applicationServices">The current application's services</param>
    public FileSystemDatabaseProviderPluginBoostrapper(IServiceProvider applicationServices)
    {
        this.ApplicationServices = applicationServices;
    }

    /// <summary>
    /// Gets the current application's services
    /// </summary>
    protected IServiceProvider ApplicationServices { get; }

    /// <inheritdoc/>
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(this.ApplicationServices.GetRequiredService<IConfiguration>());
        services.AddSingleton(this.ApplicationServices.GetRequiredService<ILoggerFactory>());
        services.AddSingleton<FileSystemDatabase>();
    }

}
