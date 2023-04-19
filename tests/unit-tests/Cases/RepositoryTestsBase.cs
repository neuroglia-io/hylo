using Hylo.Infrastructure;
using Hylo.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.UnitTests.Cases;

[TestCaseOrderer("Xunit.PriorityTestCaseOrderer", "Hylo.UnitTests")]
public abstract class RepositoryTestsBase
    : IDisposable
{

    internal const string FakeNamespaceName = "fake-namespace";
    bool _disposed;
    readonly ResourceRepositoryOptionsBuilder _builder;

    protected RepositoryTestsBase(Action<IResourceRepositoryOptionsBuilder> setup)
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddHttpClient();
        services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor());
        this._builder = new(configuration, services);
        setup(this._builder);
    }

    [Fact, Priority(1)]
    public async Task Add_Resource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        using var resources = await this._builder
            .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync()
            .ConfigureAwait(false);
        var name = "fake-resource-1";
        var labels = new Dictionary<string, string>() { { "fake-label-key-1", "fake-label-value-1" } };
        var spec = new FakeResourceSpec();
        var status = new FakeResourceStatus();
        var resource = new FakeResourceWithSpecAndStatus(new ResourceMetadata(name, @namespace, labels), spec, status);
        var resourceNode = Serializer.Json.SerializeToNode(resource)!.AsObject();
        resourceNode.Remove(nameof(IMetadata.Metadata).ToCamelCase());
        var json = Serializer.Json.Serialize(resourceNode);

        //act
        var persistedResource = await resources.AddAsync(resource).ConfigureAwait(false);

        //assert
        persistedResource.Should().NotBeNull();
        persistedResource.Metadata.Name.Should().Be(name);
        persistedResource.Metadata.Namespace.Should().Be(@namespace);
        persistedResource.Metadata.CreationTimestamp.Should().NotBeNull();
        persistedResource.Metadata.Generation.Should().Be(1);
        persistedResource.Metadata.ResourceVersion.Should().NotBeNullOrWhiteSpace();
        persistedResource.Spec.Should().BeEquivalentTo(spec);
        persistedResource.Status.Should().BeEquivalentTo(status);
    }

    [Fact, Priority(2)]
    public async Task Get_ResourceDefinition_Should_Work()
    {
        //arrange
        using var resources = await this._builder
            .BuildAsync()
            .ConfigureAwait(false); ;

        //act
        var namespaceDefinition = await resources.GetDefinitionAsync(NamespaceDefinition.ResourceGroup, NamespaceDefinition.ResourcePlural);

        //assert
        namespaceDefinition.Should().NotBeNull();
    }

    [Fact, Priority(3)]
    public async Task List_ResourcesShould_Work()
    {
        //arrange
        var resourceCount = 10;
        var @namespace = FakeNamespaceName;
        var resources = new List<FakeResourceWithSpecAndStatus>(resourceCount);
        var repository = await this._builder
            .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync()
            .ConfigureAwait(false);
        FakeResourceWithSpecAndStatus.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
            resource = await repository.AddAsync(resource).ConfigureAwait(false);
            resource.Metadata.ExtensionData = null;
            resources.Add(resource);
        }

        //act
        var collection = await repository.ListAsync<FakeResourceWithSpecAndStatus>(@namespace);

        //assert
        collection.Should().NotBeNull();
        collection.Items.Should().NotBeNull();
        collection.Items!.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(resources);
    }

    [Fact, Priority(4)]
    public async Task ListAndFilter_Resources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var @namespace = FakeNamespaceName;
        var resources = new List<FakeResourceWithSpecAndStatus>(resourceCount);
        var repository = await this._builder
            .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync()
            .ConfigureAwait(false);
        FakeResourceWithSpecAndStatus.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeResourceWithSpecAndStatus.Create(@namespace, new Dictionary<string, string>() { { "fake-label", $"{(i < 5 ? "foo" : i < 8 ? "bar" : "baz")}" } });
            resource = await repository.AddAsync(resource).ConfigureAwait(false);
            resources.Add(resource);
        }

        //act
        var fooCollection = await repository.ListAsync<FakeResourceWithSpecAndStatus>(@namespace, new LabelSelector[] { new("fake-label", LabelSelectionOperator.Equals, "foo") });
        var barCollection = await repository.ListAsync<FakeResourceWithSpecAndStatus>(@namespace, new LabelSelector[] { new("fake-label", LabelSelectionOperator.Contains, "bar") });
        var bazCollection = await repository.ListAsync<FakeResourceWithSpecAndStatus>(@namespace, new LabelSelector[] { new("fake-label", LabelSelectionOperator.NotContains, "foo", "bar") });
        var barBazCollection = await repository.ListAsync<FakeResourceWithSpecAndStatus>(@namespace, new LabelSelector[] { new("fake-label", LabelSelectionOperator.Contains, "bar", "baz") });
        var notFooCollection = await repository.ListAsync<FakeResourceWithSpecAndStatus>(@namespace, new LabelSelector[] { new("fake-label", LabelSelectionOperator.NotEquals, "foo") });

        //assert
        fooCollection.Should().NotBeNull();
        fooCollection.Items.Should().NotBeNull();
        fooCollection.Items.Should().HaveCount(5);

        barCollection.Should().NotBeNull();
        barCollection.Items.Should().NotBeNull();
        barCollection.Items.Should().HaveCount(3);

        bazCollection.Should().NotBeNull();
        bazCollection.Items.Should().NotBeNull();
        bazCollection.Items.Should().HaveCount(2);

        barBazCollection.Should().NotBeNull();
        barBazCollection.Items.Should().NotBeNull();
        barBazCollection.Items.Should().HaveCount(5);

        notFooCollection.Items!.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(barBazCollection.Items!.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }));
    }

    [Fact, Priority(5)]
    public async Task GetAll_Resources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var @namespace = FakeNamespaceName;
        var resources = new List<FakeResourceWithSpecAndStatus>(resourceCount);
        var repository = await this._builder
            .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync()
            .ConfigureAwait(false);
        FakeResourceWithSpecAndStatus.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
            resource = await repository.AddAsync(resource).ConfigureAwait(false);
            resource.Metadata.ExtensionData = null;
            resources.Add(resource);
        }

        //act
        var collection = repository.GetAllAsync<FakeResourceWithSpecAndStatus>(@namespace);
        var items = await collection.ToListAsync().ConfigureAwait(false);

        //assert
        collection.Should().NotBeNull();
        items.Should().NotBeNull();
        items.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(resources);
    }

    [Fact, Priority(6)]
    public async Task Watch_Resources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var @namespace = FakeNamespaceName;
        using var resourceRepository = await this._builder
             .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
             .WithResource(new Namespace(@namespace))
             .BuildAsync()
             .ConfigureAwait(false);

        var processedWatchEvents = new List<IResourceWatchEvent>(resourceCount);

        //act
        await using var watch = await resourceRepository.WatchAsync<FakeResourceWithSpecAndStatus>(@namespace).ConfigureAwait(false);
        watch.Subscribe(processedWatchEvents.Add);
        FakeResourceWithSpecAndStatus.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            await resourceRepository.AddAsync(FakeResourceWithSpecAndStatus.Create(@namespace)).ConfigureAwait(false);
        }
        await Task.Delay(100);

        //assert
        processedWatchEvents.ToList().Should().HaveCount(resourceCount);
    }

    [Fact, Priority(7)]
    public async Task Get_Resource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        using var resources = await this._builder
             .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
             .WithResource(new Namespace(@namespace))
             .BuildAsync()
             .ConfigureAwait(false);
        var resource = await resources.AddAsync(FakeResourceWithSpecAndStatus.Create(@namespace)).ConfigureAwait(false);
        resource.Metadata.ExtensionData = null;

        //act
        var fetchedResource = await resources.GetAsync<FakeResourceWithSpecAndStatus>(resource.GetName(), resource.GetNamespace()).ConfigureAwait(false);

        //assert
        fetchedResource.Should().NotBeNull();
        fetchedResource!.Metadata.ExtensionData = null;
        fetchedResource.Should().BeEquivalentTo(resource);
    }

    [Fact, Priority(8)]
    public async Task Patch_Resource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
        using var resources = await this._builder
             .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
             .WithResource(new Namespace(@namespace))
             .WithResource(resource)
             .BuildAsync()
             .ConfigureAwait(false);
        var updatedResource = resource.Clone()!;
        updatedResource.Spec.FakeProperty1 = "Updated Fake Value";
        updatedResource.Spec.FakeProperty2 = 6;
        updatedResource.Status!.FakeProperty1 = "Updated Fake Value";
        updatedResource.Status!.FakeProperty2 = 6;
        var jsonPatch = JsonPatchHelper.CreateJsonPatchFromDiff(resource, updatedResource);
        var patch = new Patch(PatchType.JsonPatch, jsonPatch);

        //act
        var patchedResource = await resources.PatchAsync<FakeResourceWithSpecAndStatus>(patch, resource.GetName(), resource.GetNamespace()).ConfigureAwait(false);

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().NotBe(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
    }

    [Fact, Priority(9)]
    public async Task Update_Resource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
        using var resources = await this._builder
            .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
            .WithResource(new Namespace(@namespace))
            .WithResource(resource)
            .BuildAsync()
            .ConfigureAwait(false);
        var updatedResource = resource.Clone()!;
        updatedResource.Spec.FakeProperty1 = "Updated Fake Value";
        updatedResource.Spec.FakeProperty2 = 6;
        updatedResource.Status!.FakeProperty1 = "Updated Fake Value";
        updatedResource.Status!.FakeProperty2 = 6;

        //act
        var patchedResource = await resources.UpdateAsync(updatedResource).ConfigureAwait(false);

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().NotBe(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
    }

    [Fact, Priority(10)]
    public async Task Patch_ResourceStatus_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
        using var resources = await this._builder
             .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
             .WithResource(new Namespace(@namespace))
             .WithResource(resource)
             .BuildAsync()
             .ConfigureAwait(false);
        var updatedResource = resource.Clone()!;
        updatedResource.Spec.FakeProperty1 = "Updated Fake Value";
        updatedResource.Spec.FakeProperty2 = 6;
        updatedResource.Status!.FakeProperty1 = "Updated Fake Value";
        updatedResource.Status!.FakeProperty2 = 6;
        var jsonPatch = JsonPatchHelper.CreateJsonPatchFromDiff(resource, updatedResource);
        var patch = new Patch(PatchType.JsonPatch, jsonPatch);

        //act
        var patchedResource = await resources.PatchStatusAsync<FakeResourceWithSpecAndStatus>(patch, resource.GetName(), resource.GetNamespace()).ConfigureAwait(false);

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().Be(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);

    }

    [Fact, Priority(11)]
    public async Task Update_ResourceStatus_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
        using var resources = await this._builder
             .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
             .WithResource(new Namespace(@namespace))
             .WithResource(resource)
             .BuildAsync()
             .ConfigureAwait(false);
        var updatedResource = resource.Clone()!;
        updatedResource.Spec.FakeProperty1 = "Updated Fake Value";
        updatedResource.Spec.FakeProperty2 = 6;
        updatedResource.Status!.FakeProperty1 = "Updated Fake Value";
        updatedResource.Status!.FakeProperty2 = 6;

        //act
        var patchedResource = await resources.UpdateStatusAsync(updatedResource).ConfigureAwait(false);

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().Be(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);
    }

    [Fact, Priority(12)]
    public async Task Delete_Resource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
        using var resources = await this._builder
             .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
             .WithResource(new Namespace(@namespace))
             .WithResource(resource)
             .BuildAsync()
             .ConfigureAwait(false);

        //act
        var deletedResource = await resources.RemoveAsync<FakeResourceWithSpecAndStatus>(resource.GetName(), resource.GetNamespace());

        //assert
        deletedResource.Should().NotBeNull();
        deletedResource.Should().BeEquivalentTo(resource);
        (await resources.GetAsync<FakeResourceWithSpecAndStatus>(resource.GetName(), resource.GetNamespace())).Should().BeNull();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {

            }
            this._disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}