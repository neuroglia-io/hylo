using Hylo.Providers.FileSystem;

namespace Hylo.UnitTests.Cases.Providers;

public class FileSystemProviderTests
    : RepositoryTestsBase
{

    static readonly string ConnectionString = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToShortString());

    public FileSystemProviderTests() 
        : base(builder => builder.UseFileSystem(ConnectionString))
    {

    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        IResourceRepositoryBuilderExtensions.ServiceProvider.Dispose();
        Task.Delay(10).GetAwaiter().GetResult();
        Directory.Delete(ConnectionString, true);
    }

}
