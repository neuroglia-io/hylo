using Hylo.Infrastructure;
using Hylo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.Providers.Redis.Services;

/// <summary>
/// Represents the <see cref="IPluginBootstrapper"/> implementation used to configure the <see cref="RedisDatabaseProvider"/> plugin
/// </summary>
public class RedisDatabaseProviderPluginBootstrapper
    : IPluginBootstrapper
{

    /// <summary>
    /// Initializes a new <see cref="RedisDatabaseProviderPluginBootstrapper"/>
    /// </summary>
    /// <param name="applicationServices">The current application's services</param>
    public RedisDatabaseProviderPluginBootstrapper(IServiceProvider applicationServices)
    {
        this.ApplicationServices = applicationServices;
    }

    /// <summary>
    /// Gets the current application's services
    /// </summary>
    protected IServiceProvider ApplicationServices { get; }

    /// <inheritdoc/>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddStackExchangeRedis(this.ApplicationServices.GetRequiredService<IConfiguration>().GetConnectionString(RedisDatabase.ConnectionStringName)!);
        services.AddSingleton<RedisDatabase>();
    }

}
