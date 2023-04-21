using Hylo.Infrastructure.Services;
using Hylo.Providers.Kubernetes.Services;
using k8s.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.Providers.Kubernetes;

/// <summary>
/// Defines extensions for <see cref="IRepositoryOptionsBuilder"/>s
/// </summary>
public static class IResourceRepositoryOptionsBuilderExtensions
{

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to use the <see cref="K8sResourceStorageProvider"/>
    /// </summary>
    /// <param name="builder">The <see cref="IRepositoryOptionsBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    public static IRepositoryOptionsBuilder UseKubernetes(this IRepositoryOptionsBuilder builder)
    {
        builder.Services.AddKubernetesClient();
        builder.Services.AddSingleton<K8sResourceStorage>();
        builder.UseDefinitionsKind(V1CustomResourceDefinition.KubeGroup, V1CustomResourceDefinition.KubeApiVersion, V1CustomResourceDefinition.KubePluralName, V1CustomResourceDefinition.KubeKind);
        builder.UseNamespacesKind(V1Namespace.KubeGroup, V1Namespace.KubeApiVersion, V1Namespace.KubePluralName, V1Namespace.KubeKind);
        builder.UseStorageProvider<K8sResourceStorageProvider>();
        return builder;
    }

}