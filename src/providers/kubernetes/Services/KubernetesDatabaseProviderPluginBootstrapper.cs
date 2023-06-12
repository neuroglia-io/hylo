using Hylo.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.Providers.Kubernetes.Services;

/// <summary>
/// Represents the <see cref="IPluginBootstrapper"/> implementation used to configure the <see cref="KubernetesDatabaseProvider"/> plugin
/// </summary>
public class KubernetesDatabaseProviderPluginBootstrapper
    : IPluginBootstrapper
{
    
    /// <inheritdoc/>
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddKubernetesClient();
        services.AddSingleton<KubernetesDatabase>();
    }

}
