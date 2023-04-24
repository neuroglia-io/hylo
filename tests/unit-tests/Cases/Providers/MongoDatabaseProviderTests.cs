using Hylo.Providers.Mongo;
using Hylo.UnitTests.Containers;

namespace Hylo.UnitTests.Cases.Providers;

public class MongoDatabaseProviderTests
    : RepositoryTestsBase
{

    public MongoDatabaseProviderTests()
        : base(builder => builder.UseMongo(MongoContainer.ConnectionString))
    {

    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        base.Dispose(disposing);
        MongoContainer.DisposeAsync().GetAwaiter().GetResult();
    }

}
