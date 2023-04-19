using k8s;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.Providers.Kubernetes;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures the Kubernets API client
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddKubernetesClient(this IServiceCollection services)
    {
        services.AddSingleton(provider => new k8s.Kubernetes(ApplicationInfo.RunsInKubernetes ? KubernetesClientConfiguration.InClusterConfig() : KubernetesClientConfiguration.BuildConfigFromConfigFile()));
        return services;
    }

}
