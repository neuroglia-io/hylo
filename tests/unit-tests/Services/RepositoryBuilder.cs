using Hylo.Infrastructure;
using Hylo.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hylo.UnitTests.Services;

public class RepositoryBuilder
    : IDisposable
{

    bool _disposed;

    public RepositoryBuilder(Action<IRepositoryOptionsBuilder> optionsSetup)
    {
        this.OptionsSetup = optionsSetup;
    }

    protected Action<IRepositoryOptionsBuilder> OptionsSetup { get; }

    protected List<IResourceDefinition> Definitions { get; } = new();

    protected List<IResource> Resources { get; } = new();

    public ServiceProvider ServiceProvider { get; private set; } = null!;

    public RepositoryBuilder WithDefinition<TDefinition>()
        where TDefinition : class, IResourceDefinition, new()
    {
        this.Definitions.Add(new TDefinition());
        return this;
    }

    public RepositoryBuilder WithResource<TResource>(TResource resource)
        where TResource : class, IResource, new()
    {
        this.Resources.Add(resource);
        return this;
    }

    public async Task<IRepository> BuildAsync(CancellationToken cancellationToken = default)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddLogging();
        services.AddHttpClient();
        services.AddHylo(configuration, this.OptionsSetup);
        this.ServiceProvider = services.BuildServiceProvider();

        foreach (var hostedService in ServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        var repository = this.ServiceProvider.GetRequiredService<IRepository>();

        foreach(var definition in this.Definitions)
        {
            await repository.AddAsync(definition.ConvertTo<ResourceDefinition>()!, false, cancellationToken).ConfigureAwait(false);
        }

        foreach (var resource in this.Resources)
        {
            await repository.AddAsync(resource, resource.GetGroup(), resource.GetVersion(), resource.Definition.Plural, false, cancellationToken).ConfigureAwait(false);
        }

        return repository;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this.ServiceProvider?.Dispose();
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
