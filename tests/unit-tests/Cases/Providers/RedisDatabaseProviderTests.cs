using Hylo.Providers.Redis;
using Hylo.UnitTests.Containers;

namespace Hylo.UnitTests.Cases.Providers;

public class RedisDatabaseProviderTests
    : DatabaseTestsBase
{

    public RedisDatabaseProviderTests()
        : base(builder => builder.UseRedis(RedisContainer.ConnectionString))
    {

    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        base.Dispose(disposing);
        RedisContainer.DisposeAsync().GetAwaiter().GetResult();
    }

}
