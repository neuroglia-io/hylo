﻿using Hylo.Infrastructure;
using Hylo.Infrastructure.Services;
using Hylo.Providers.FileSystem.Services;
using Hylo.UnitTests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.UnitTests.Cases;

[TestCaseOrderer("Xunit.PriorityTestCaseOrderer", "Hylo.UnitTests")]
public class PluginTests
    : IDisposable
{

    static readonly DirectoryInfo PluginsDirectory = new(Path.Combine(AppContext.BaseDirectory, "plugins"));

    [Fact, Priority(1)]
    public async Task Find_AssemblyBasedPlugin_Should_Work()
    {
        //arrange
        if (PluginsDirectory.Exists) PluginsDirectory.Delete(true);
        PluginsDirectory.Create();
        var assemblyFileLocation = Path.Combine(PluginsDirectory.FullName, $"{typeof(FakeDataGeneratorPlugin).Assembly.GetName().Name}.dll");
        File.Copy(typeof(FakeDataGeneratorPlugin).Assembly.Location, assemblyFileLocation);
        var metadata = new PluginMetadata(assemblyFileLocation, typeof(FakeDataGeneratorPlugin).FullName!, typeof(FakeDataGeneratorPluginBootstrapper).FullName!);
        File.WriteAllText(Path.Combine(PluginsDirectory.FullName, "fake.plugin.json"), Serializer.Json.Serialize(metadata));
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        using var serviceProvider = services.BuildServiceProvider();
        var manager = new PluginManager(serviceProvider);
        await manager.StartAsync(default).ConfigureAwait(false);

        //act
        var plugin = await manager.FindPluginAsync<IFakeDataGenerator>().ConfigureAwait(false);

        //assert
        plugin.Should().NotBeNull();
        plugin!.GenerateFakeData().Should().Be(FakeDataGeneratorPluginBootstrapper.InjectedString);
    }

    [Fact(Skip = "Pending the publish of a package using latest plugin types"), Priority(2)]
    public async Task Find_PackageBasedPlugin_Should_Work()
    {
        //arrange
        if (PluginsDirectory.Exists) PluginsDirectory.Delete(true);
        PluginsDirectory.Create();
        var assemblyFileLocation = Path.Combine(PluginsDirectory.FullName, new FileInfo(typeof(FileSystemDatabaseProvider).Assembly.Location).Name);
        var metadata = new PluginMetadata(assemblyFileLocation, typeof(FileSystemDatabaseProvider).FullName!, nugetPackage: typeof(FileSystemDatabaseProvider).Assembly.GetName().Name);
        File.WriteAllText(Path.Combine(PluginsDirectory.FullName, "fake.plugin.json"), Serializer.Json.Serialize(metadata));
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        using var serviceProvider = services.BuildServiceProvider();
        var manager = new PluginManager(serviceProvider);
        await manager.StartAsync(default).ConfigureAwait(false);

        //act
        var plugin = await manager.FindPluginAsync<IRepository>().ConfigureAwait(false);

        //assert
        plugin.Should().NotBeNull();
    }

    void IDisposable.Dispose()
    {
        if (PluginsDirectory.Exists)
        {
            while (true)
            {
                try
                {
                    PluginsDirectory.Delete(true);
                    break;
                }
                catch (IOException ex) when(ex is not FileNotFoundException) { }
            }
        }
        GC.SuppressFinalize(this);
    }

}