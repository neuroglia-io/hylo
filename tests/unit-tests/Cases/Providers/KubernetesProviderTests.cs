using Hylo.Providers.Kubernetes;
using k8s;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.UnitTests.Cases.Providers;

public class KubernetesProviderTests
    : RepositoryTestsBase
{

    public KubernetesProviderTests() 
        : base(builder => builder.UseKubernetes())
    {

    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        var k8s = IResourceRepositoryBuilderExtensions.ServiceProvider!.GetRequiredService<Kubernetes>();
        try
        {
            k8s.DeleteNamespace(RepositoryTestsBase.FakeNamespaceName);
        }
        catch { }
        try
        {
            k8s.DeleteCustomResourceDefinition($"{FakeResourceWithSpecAndStatusDefinition.ResourcePlural}.{FakeResourceWithSpecAndStatusDefinition.ResourceGroup}");
        }
        catch { }
        Task.Delay(1000).GetAwaiter().GetResult();
        IResourceRepositoryBuilderExtensions.ServiceProvider!.Dispose();
    }

}
