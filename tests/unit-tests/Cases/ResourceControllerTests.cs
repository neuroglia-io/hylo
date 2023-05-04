using Hylo.Infrastructure.Configuration;
using Hylo.Infrastructure.Services;
using Hylo.Providers.FileSystem;
using Hylo.UnitTests.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Hylo.UnitTests.Cases;

public sealed class ResourceControllerTests
    : IDisposable
{

    static readonly string ConnectionString = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToShortString());
    const string FakeNamespaceName = "fake-namespace";
    readonly RepositoryBuilder _repositoryBuilder = new(builder => builder.UseFileSystem(ConnectionString));
    IRepository _repository = null!;

    async Task<IResourceController<TResource>> BuildControllerAsync<TResource>(Action<RepositoryBuilder> repositorySetup, ResourceControllerOptions<TResource>? options = null)
        where TResource : class, IResource, new()
    {
        options ??= new();
        repositorySetup(_repositoryBuilder);
        _repository = await this._repositoryBuilder.BuildAsync().ConfigureAwait(false);
        var controller = new ResourceController<TResource>(new NullLoggerFactory(), Options.Create(options), _repository);
        await controller.StartAsync(default).ConfigureAwait(false);
        return controller;
    }

    [Fact, Priority(1)]
    public async Task Controller_For_NamespacedResources_Should_ReconcileOnStart()
    {
        using var controller = await BuildControllerAsync<FakeNamespacedResource>(
            repository => repository
                .WithDefinition<FakeNamespacedResourceDefinition>()
                .WithResource(new Namespace(FakeNamespaceName))
                .WithResource(FakeNamespacedResource.Create(FakeNamespaceName))
                .WithResource(FakeNamespacedResource.Create(FakeNamespaceName))
                .WithResource(FakeNamespacedResource.Create(FakeNamespaceName))
                .WithResource(FakeNamespacedResource.Create(FakeNamespaceName))
                .WithResource(FakeNamespacedResource.Create(FakeNamespaceName))
                .WithResource(FakeNamespacedResource.Create(Namespace.DefaultNamespaceName)),
            new() { ResourceNamespace = FakeNamespaceName })
            .ConfigureAwait(false);

        //act
        var resources = await _repository.GetAllAsync<FakeNamespacedResource>(FakeNamespaceName).ToListAsync().ConfigureAwait(false);

        //assert
        controller.Resources.Count.Should().Be(5);
        controller.Resources.Should().BeEquivalentTo(resources);
    }

    [Fact, Priority(2)]
    public async Task Controller_For_ClusterResources_Should_ReconcileOnStart()
    {
        using var controller = await BuildControllerAsync<FakeClusterResource>(repository =>
            repository
                .WithDefinition<FakeClusterResourceDefinition>()
                .WithResource(FakeClusterResource.Create())
                .WithResource(FakeClusterResource.Create())
                .WithResource(FakeClusterResource.Create())
                .WithResource(FakeClusterResource.Create())
                .WithResource(FakeClusterResource.Create()))
            .ConfigureAwait(false);

        //act
        var resources = await _repository.GetAllAsync<FakeClusterResource>().ToListAsync().ConfigureAwait(false);

        //assert
        controller.Resources.Count.Should().Be(5);
        controller.Resources.Should().BeEquivalentTo(resources);
    }

    public void Dispose()
    {
        _repositoryBuilder?.Dispose();
        Task.Delay(10).GetAwaiter().GetResult();
        Directory.Delete(ConnectionString, true);
    }

}