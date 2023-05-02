using Hylo.Infrastructure.Services;
using Hylo.Providers.FileSystem;
using Hylo.UnitTests.Services;

namespace Hylo.UnitTests.Cases;

public class RepositoryTests
    : IDisposable
{

    static readonly string ConnectionString = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToShortString());

    internal const string FakeNamespaceName = "fake-namespace";

    public RepositoryTests()
    {
        this.RepositoryBuilder = new(builder => builder.UseFileSystem(ConnectionString));
    }

    public RepositoryBuilder RepositoryBuilder { get; }

    [Fact, Priority(1)]
    public async Task Create_NamespacedResource_With_No_Namespace_Should_Default()
    {
        //arrange
        using var resourceRepository = await this.RepositoryBuilder
           .WithDefinition<FakeNamespacedResourceDefinition>()
           .BuildAsync()
           .ConfigureAwait(false);

        //act
        var resource = await resourceRepository.AddAsync(new FakeNamespacedResource(new("test-1"), new(), new()));

        //assert
        resource.Metadata.Namespace.Should().Be(Namespace.DefaultNamespaceName);
    }

    [Fact, Priority(2)]
    public async Task Create_ClusterResource_With_Namespace_Should_Fail()
    {
        //arrange
        using var resourceRepository = await this.RepositoryBuilder
           .WithDefinition<FakeClusterResourceDefinition>()
           .BuildAsync()
           .ConfigureAwait(false);
        var resource = FakeClusterResource.Create();
        resource.Metadata.Namespace = FakeNamespaceName;

        //act
        var test = async () => await resourceRepository.AddAsync(resource);

        //assert
        await test.Should().ThrowAsync<HyloException>();
    }

    [Fact, Priority(3)]
    public async Task Get_NamespacedResource_Should_Work()
    {
        //arrange
        using var resourceRepository = await this.RepositoryBuilder
           .WithDefinition<FakeNamespacedResourceDefinition>()
           .WithResource(new Namespace(FakeNamespaceName))
           .BuildAsync()
           .ConfigureAwait(false);
        var resource = await resourceRepository.AddAsync(new FakeNamespacedResource(new("fake", FakeNamespaceName), new FakeResourceSpec() { FakeProperty1 = "Fake Property 1", FakeProperty2 = 1 }));

        //act
        var persistedResource = await resourceRepository.GetAsync<FakeNamespacedResource>(resource.GetName(), resource.GetNamespace());

        //assert
        persistedResource.Should().NotBeNull();
        persistedResource!.Spec.Should().BeEquivalentTo(resource.Spec);
    }

    [Fact, Priority(4)]
    public async Task Get_ClusterResource_Should_Work()
    {
        //arrange
        using var resourceRepository = await this.RepositoryBuilder
           .WithDefinition<FakeClusterResourceDefinition>()
           .BuildAsync()
           .ConfigureAwait(false);
        var resource = await resourceRepository.AddAsync(FakeClusterResource.Create());

        //act
        var persistedResource = await resourceRepository.GetAsync<FakeClusterResource>(resource.GetName());

        //assert
        persistedResource.Should().NotBeNull();
        persistedResource!.Spec.Should().BeEquivalentTo(resource.Spec);
    }

    [Fact, Priority(10)]
    public async Task Monitor_LeaveWatchOpen_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        using var resourceRepository = await this.RepositoryBuilder
            .WithDefinition<FakeNamespacedResourceDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync()
            .ConfigureAwait(false);
        var resource = await resourceRepository.AddAsync(FakeNamespacedResource.Create(@namespace)).ConfigureAwait(false);
        var watch = await resourceRepository.WatchAsync<FakeNamespacedResource>(@namespace).ConfigureAwait(false);
        var monitor = new ResourceMonitor<FakeNamespacedResource>(watch, resource, true);

        //act
        await monitor.DisposeAsync().ConfigureAwait(false);

        //assert
        var test = () => new ResourceMonitor<FakeNamespacedResource>(watch, resource, true);
        test.Should().NotThrow();

    }

    public void Dispose()
    {
        Task.Delay(10).GetAwaiter().GetResult();
        Directory.Delete(ConnectionString, true);
        GC.SuppressFinalize(this);
    }

}
