using Hylo.Infrastructure.Services;
using Hylo.Resources.Definitions;
using k8s.Models;
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
        services.AddSingleton<KubernetesDatabase>();

        ResourceDefinition.ResourceGroup = V1CustomResourceDefinition.KubeGroup;
        ResourceDefinition.ResourceVersion = V1CustomResourceDefinition.KubeApiVersion;
        ResourceDefinition.ResourcePlural = V1CustomResourceDefinition.KubePluralName;
        ResourceDefinition.ResourceKind = V1CustomResourceDefinition.KubeKind;

        NamespaceDefinition.ResourceGroup = V1Namespace.KubeGroup;
        NamespaceDefinition.ResourceVersion = V1Namespace.KubeApiVersion;
        NamespaceDefinition.ResourcePlural = V1Namespace.KubePluralName;
        NamespaceDefinition.ResourceKind = V1Namespace.KubeKind;
    }

}
