using Hylo.Infrastructure;
using Hylo.Infrastructure.Services;
using Hylo.Providers.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hylo.Providers.Redis;

/// <summary>
/// Defines extensions for <see cref="IRepositoryOptionsBuilder"/>s
/// </summary>
public static class IRepositoryOptionsBuilderExtensions
{

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to use the <see cref="RedisDatabaseProvider"/>
    /// </summary>
    /// <param name="builder">The <see cref="IRepositoryOptionsBuilder"/> to configure</param>
    /// <param name="connectionString">The Mongo DB connection string</param>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    public static IRepositoryOptionsBuilder UseRedis(this IRepositoryOptionsBuilder builder, string? connectionString = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) connectionString = builder.Configuration.GetConnectionString(RedisDatabase.ConnectionStringName)!;
        builder.Services.AddStackExchangeRedis(connectionString);
        builder.Services.AddSingleton<RedisDatabase>();
        builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<RedisDatabase>());
        builder.UseDatabaseProvider<RedisDatabaseProvider>();
        return builder;
    }

}