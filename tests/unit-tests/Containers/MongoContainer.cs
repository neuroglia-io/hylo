using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Hylo.UnitTests.Containers;

public static class MongoContainer
{

    static IContainer? Container;
    const int PublicPort = 27017;

    public static string ConnectionString => $"mongodb://test:test@localhost:{Build().GetMappedPublicPort(PublicPort)}";

    public static IContainer Build()
    {
        if (Container != null) return Container;
        Container = new ContainerBuilder()
            .WithName($"mongo-{Guid.NewGuid():N}")
            .WithImage("mongo:6.0.3")
            .WithBindMount(Path.Combine(AppContext.BaseDirectory, "assets", "mongo", "replica.key"), "/data/replica.key.tmp")
            .WithBindMount(Path.Combine(AppContext.BaseDirectory, "assets", "mongo", "mongo-init.sh"), "/docker-entrypoint-initdb.d/mongo-init.sh")
            .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "test")
            .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "test")
            .WithEntrypoint(
                "bash", 
                "-c",
                "cp /data/replica.key.tmp /data/replica.key\nchmod 400 /data/replica.key\nchown 999:999 /data/replica.key\nexec docker-entrypoint.sh $@     \n")
             .WithCommand(
                    "--bind_ip_all",
                    "--replSet",
                    "rs0",
                    "--keyFile",
                    "/data/replica.key"
                )
            .WithPortBinding(PublicPort, true)
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilPortIsAvailable(PublicPort))
            .Build();
        Container.StartAsync().GetAwaiter().GetResult();
        return Container;
    }

    public static async ValueTask DisposeAsync()
    {
        if (Container == null) return;
        await Container.DisposeAsync().ConfigureAwait(false);
        Container = null;
    }

}
