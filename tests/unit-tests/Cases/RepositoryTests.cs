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
    public async Task Monitor_LeaveWatchOpen_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        using var resourceRepository = await this.RepositoryBuilder
            .WithDefinition<FakeResourceWithSpecAndStatusDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync()
            .ConfigureAwait(false);
        var resource = await resourceRepository.AddAsync(FakeResourceWithSpecAndStatus.Create(@namespace)).ConfigureAwait(false);
        var watch = await resourceRepository.WatchAsync<FakeResourceWithSpecAndStatus>(@namespace).ConfigureAwait(false);
        var monitor = new ResourceMonitor<FakeResourceWithSpecAndStatus>(watch, resource, true);

        //act
        await monitor.DisposeAsync().ConfigureAwait(false);

        //assert
        var test = () => new ResourceMonitor<FakeResourceWithSpecAndStatus>(watch, resource, true);
        test.Should().NotThrow();

    }

    public void Dispose()
    {
        Task.Delay(10).GetAwaiter().GetResult();
        Directory.Delete(ConnectionString, true);
        GC.SuppressFinalize(this);
    }

}
