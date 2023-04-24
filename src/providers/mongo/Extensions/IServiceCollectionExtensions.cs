using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

namespace Hylo.Providers;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Registers and configures an <see cref="IMongoClient"/> service
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="connectionString">The Mongo connection string to use</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMongoClient(this IServiceCollection services, string connectionString)
    {
        services.TryAddSingleton<IMongoClient>(provider => new MongoClient(MongoClientSettings.FromConnectionString(connectionString)));
        return services;
    }

}
