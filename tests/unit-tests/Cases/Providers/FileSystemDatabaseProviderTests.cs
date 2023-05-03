using Hylo.Providers.FileSystem;

namespace Hylo.UnitTests.Cases.Providers;

public class FileSystemDatabaseProviderTests
    : DatabaseTestsBase
{

    static readonly string ConnectionString = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToShortString());

    public FileSystemDatabaseProviderTests() 
        : base(builder => builder.UseFileSystem(ConnectionString))
    {

    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        base.Dispose(disposing);
        Task.Delay(5).GetAwaiter().GetResult();
        Directory.Delete(ConnectionString, true);
    }

}
