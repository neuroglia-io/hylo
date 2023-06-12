using Hylo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.Providers.Mongo.Services;

/// <summary>
/// Represents the <see cref="IPluginBootstrapper"/> implementation used to configure the <see cref="MongoDatabaseProvider"/> plugin
/// </summary>
public class MongoDatabaseProviderPluginBootstrapper
    : IPluginBootstrapper
{

    /// <summary>
    /// Initializes a new <see cref="MongoDatabaseProviderPluginBootstrapper"/>
    /// </summary>
    /// <param name="applicationServices">The current application's services</param>
    public MongoDatabaseProviderPluginBootstrapper(IServiceProvider applicationServices)
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
        services.AddLogging();
        services.AddMongoClient(this.ApplicationServices.GetRequiredService<IConfiguration>().GetConnectionString(MongoDatabase.ConnectionStringName)!);
        services.AddSingleton<MongoDatabase>();
    }

}