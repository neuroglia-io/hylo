using Hylo.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

    /// <summary>
    /// Adds and configures a new <see cref="IResourceController{TResource}"/> to control <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to control</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="IResourceController{TResource}"/> to register</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddResourceController<TResource>(this IServiceCollection services, Action<ResourceControllerOptions<TResource>>? setup = null)
        where TResource : class, IResource, new()
    {
        if (setup == null) setup = options => { };
        services.Configure(setup);
        services.TryAddSingleton<ResourceController<TResource>>();
        services.TryAddSingleton<IResourceController<TResource>>(provider => provider.GetRequiredService<ResourceController<TResource>>());
        services.AddHostedService(provider => provider.GetRequiredService<ResourceController<TResource>>());
        return services;
    }

}
