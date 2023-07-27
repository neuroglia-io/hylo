//As we failed to find an elegant solution to test Kubernetes in our CI pipeline, this test case has been commented, but it can be run on your k8s setup

using Hylo.Providers.Kubernetes;
using k8s;
using k8s.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.UnitTests.Cases.Providers;

public class KubernetesDatabaseProviderTests
    : DatabaseTestsBase
{

    public KubernetesDatabaseProviderTests()
        : base(builder => builder.UseKubernetes())
    {
        ResourceDefinition.ResourceGroup = V1CustomResourceDefinition.KubeGroup;
        ResourceDefinition.ResourceVersion = V1CustomResourceDefinition.KubeApiVersion;
        ResourceDefinition.ResourcePlural = V1CustomResourceDefinition.KubePluralName;
        ResourceDefinition.ResourceKind = V1CustomResourceDefinition.KubeKind;

        NamespaceDefinition.ResourceGroup = V1Namespace.KubeGroup;
        NamespaceDefinition.ResourceVersion = V1Namespace.KubeApiVersion;
        NamespaceDefinition.ResourcePlural = V1Namespace.KubePluralName;
        NamespaceDefinition.ResourceKind = V1Namespace.KubeKind;
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        var k8s = this.RepositoryBuilder.ServiceProvider.GetRequiredService<Kubernetes>();
        try
        {
            k8s.DeleteNamespace(FakeNamespaceName);
        }
        catch { }
        try
        {
            k8s.DeleteCustomResourceDefinition($"{MutatingWebhookDefinition.ResourcePlural}.{MutatingWebhookDefinition.ResourceGroup}");
        }
        catch { }
        try
        {
            k8s.DeleteCustomResourceDefinition($"{ValidatingWebhookDefinition.ResourcePlural}.{ValidatingWebhookDefinition.ResourceGroup}");
        }
        catch { }
        try
        {
            k8s.DeleteCustomResourceDefinition($"{FakeClusterResourceDefinition.ResourcePlural}.{FakeClusterResourceDefinition.ResourceGroup}");
        }
        catch { }
        try
        {
            k8s.DeleteCustomResourceDefinition($"{FakeNamespacedResourceDefinition.ResourcePlural}.{FakeNamespacedResourceDefinition.ResourceGroup}");
        }
        catch { }
        Task.Delay(1000).GetAwaiter().GetResult();
        base.Dispose(true);
    }

}
