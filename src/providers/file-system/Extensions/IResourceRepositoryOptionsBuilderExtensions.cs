using Hylo.Infrastructure.Services;
using Hylo.Providers.FileSystem.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hylo.Providers.FileSystem;

/// <summary>
/// Defines extensions for <see cref="IResourceRepositoryOptionsBuilder"/>s
/// </summary>
public static class IResourceRepositoryOptionsBuilderExtensions
{

    /// <summary>
    /// Configures the <see cref="IResourceRepositoryOptionsBuilder"/> to use the <see cref="FileSystemResourceStorageProvider"/>
    /// </summary>
    /// <param name="builder">The <see cref="IResourceRepositoryOptionsBuilder"/> to configure</param>
    /// <param name="connectionString">The connection string to use. Defaults to <see cref="FileSystemResourceStorage.DefaultConnectionString"/></param>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    public static IResourceRepositoryOptionsBuilder UseFileSystem(this IResourceRepositoryOptionsBuilder builder, string? connectionString = null)
    {
        builder.Configuration.GetSection("ConnectionStrings")![FileSystemResourceStorage.ConnectionStringName] = connectionString;
        builder.Services.AddSingleton<FileSystemResourceStorage>();
        builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<FileSystemResourceStorage>());
        builder.UseStorageProvider<FileSystemResourceStorageProvider>();
        return builder;
    }

}