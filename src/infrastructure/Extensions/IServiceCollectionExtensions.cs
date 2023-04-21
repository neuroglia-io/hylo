using Microsoft.Extensions.Configuration;

namespace Hylo.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures Hylo services
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="configuration">The current <see cref="IConfiguration"/></param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure Hylo</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddHylo(this IServiceCollection services, IConfiguration configuration, Action<IRepositoryOptionsBuilder> setup)
    {
        var builder = new RepositoryOptionsBuilder(configuration, services);
        setup(builder);
        builder.Build();
        return services;
    }

}
