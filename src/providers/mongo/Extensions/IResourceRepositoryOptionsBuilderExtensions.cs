using Hylo.Infrastructure.Services;
using Hylo.Providers.Mongo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hylo.Providers.Mongo;

/// <summary>
/// Defines extensions for <see cref="IRepositoryOptionsBuilder"/>s
/// </summary>
public static class IRepositoryOptionsBuilderExtensions
{

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to use the <see cref="MongoDatabaseProvider"/>
    /// </summary>
    /// <param name="builder">The <see cref="IRepositoryOptionsBuilder"/> to configure</param>
    /// <param name="connectionString">The Mongo DB connection string</param>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    public static IRepositoryOptionsBuilder UseMongo(this IRepositoryOptionsBuilder builder, string? connectionString = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) connectionString = builder.Configuration.GetConnectionString(MongoDatabase.ConnectionStringName)!;
        builder.Services.AddMongoClient(connectionString);
        builder.Services.AddSingleton<MongoDatabase>();
        builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<MongoDatabase>());
        builder.UseDatabaseProvider<MongoDatabaseProvider>();
        return builder;
    }

}