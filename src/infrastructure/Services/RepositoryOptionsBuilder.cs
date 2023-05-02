using Hylo.Infrastructure.Configuration;
using Hylo.Resources.Definitions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IRepositoryOptionsBuilder"/> interface
/// </summary>
public class RepositoryOptionsBuilder
    : IRepositoryOptionsBuilder
{

    /// <summary>
    /// Initializes a new <see cref="RepositoryOptionsBuilder"/>
    /// </summary>
    /// <param name="configuration">The current <see cref="IConfiguration"/></param>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    public RepositoryOptionsBuilder(IConfiguration configuration, IServiceCollection services)
    {
        this.Configuration = configuration;
        this.Services = services;
    }

    /// <summary>
    /// Gets the current <see cref="IConfiguration"/>
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets/sets the type of <see cref="IUserAccessor"/> to use
    /// </summary>
    protected Type UserAccessorType { get; set; } = typeof(HttpContextUserAccessor);

    /// <summary>
    /// Gets/sets the type of <see cref="IUserInfoProvider"/> to use
    /// </summary>
    protected Type UserInfoProviderType { get; set; } = typeof(UserInfoProvider);

    /// <summary>
    /// Gets/sets the type of <see cref="IAdmissionControl"/> to use
    /// </summary>
    protected Type AdmissionControlType { get; set; } = typeof(AdmissionControl);

    /// <summary>
    /// Gets/sets the type of <see cref="IPluginManager"/> to use
    /// </summary>
    protected Type PluginManagerType { get; set; } = typeof(PluginManager);

    /// <summary>
    /// Gets/sets the type of <see cref="IVersionControl"/> to use
    /// </summary>
    protected Type VersionControlType { get; set; } = typeof(VersionControl);

    /// <summary>
    /// Gets/sets the type of <see cref="IDatabaseProvider"/> to use
    /// </summary>
    protected Type DatabaseProviderType { get; set; } = typeof(PluginDatabaseProvider);

    /// <summary>
    /// Gets/sets the type of <see cref="IRepository"/> to use
    /// </summary>
    protected Type RepositoryType { get; set; } = typeof(Repository);

    /// <summary>
    /// Gets/sets the type of <see cref="IDatabaseInitializer"/> to use
    /// </summary>
    protected Type DatabaseInitializerType { get; set; } = typeof(DatabaseInitializer);

    /// <summary>
    /// Gets the <see cref="ResourceRepositoryOptions"/> to configure
    /// </summary>
    protected ResourceRepositoryOptions Options { get; } = new();

    /// <inheritdoc/>
    public virtual IRepositoryOptionsBuilder UseDefinitionsKind(string group, string version, string plural, string kind)
    {
        ObjectNamingConvention.Current.EnsureIsValidResourceGroup(group);
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        ObjectNamingConvention.Current.EnsureIsValidVersion(version);
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        ObjectNamingConvention.Current.EnsureIsValidResourcePluralName(plural);
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        ObjectNamingConvention.Current.EnsureIsValidResourceKind(kind);

        ResourceDefinition.ResourceGroup = group;
        ResourceDefinition.ResourceVersion = version;
        ResourceDefinition.ResourcePlural = plural;
        ResourceDefinition.ResourceKind = kind;

        return this;
    }

    /// <inheritdoc/>
    public virtual IRepositoryOptionsBuilder UseNamespacesKind(string group, string version, string plural, string kind)
    {
        ObjectNamingConvention.Current.EnsureIsValidResourceGroup(group);
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        ObjectNamingConvention.Current.EnsureIsValidVersion(version);
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        ObjectNamingConvention.Current.EnsureIsValidResourcePluralName(plural);
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        ObjectNamingConvention.Current.EnsureIsValidResourceKind(kind);

        NamespaceDefinition.ResourceGroup = group;
        NamespaceDefinition.ResourceVersion = version;
        NamespaceDefinition.ResourcePlural = plural;
        NamespaceDefinition.ResourceKind = kind;

        return this;
    }

    /// <inheritdoc/>
    public virtual IRepositoryOptionsBuilder WithDefaultNamespace(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        ObjectNamingConvention.Current.EnsureIsValidResourceName(name);

        Namespace.DefaultNamespaceName = name;

        return this;
    }

    IRepositoryOptionsBuilder IRepositoryOptionsBuilder.UseUserAccessor<TAccessor>()
    {
        this.UserAccessorType = typeof(TAccessor);
        return this;
    }

    IRepositoryOptionsBuilder IRepositoryOptionsBuilder.UseUserInfoProvider<TProvider>()
    {
        this.UserInfoProviderType = typeof(TProvider);
        return this;
    }

    IRepositoryOptionsBuilder IRepositoryOptionsBuilder.UsePluginManager<TManager>()
    {
        this.PluginManagerType = typeof(TManager);
        return this;
    }

    IRepositoryOptionsBuilder IRepositoryOptionsBuilder.UseAdmissionControl<TControl>()
    {
        this.AdmissionControlType = typeof(TControl);
        return this;
    }

    IRepositoryOptionsBuilder IRepositoryOptionsBuilder.UseVersionControl<TControl>()
    {
        this.VersionControlType = typeof(TControl);
        return this;
    }

    IRepositoryOptionsBuilder IRepositoryOptionsBuilder.UseDatabaseProvider<TProvider>()
    {
        this.DatabaseProviderType = typeof(TProvider);
        return this;
    }

    IRepositoryOptionsBuilder IRepositoryOptionsBuilder.UseRepository<TRepository>()
    {
        this.RepositoryType = typeof(TRepository);
        return this;
    }

    IRepositoryOptionsBuilder IRepositoryOptionsBuilder.UseDatabaseInitializer<TInitializer>()
    {
        this.DatabaseInitializerType = typeof(TInitializer);
        return this;
    }

    /// <inheritdoc/>
    public virtual void Build()
    {
        this.Services.TryAddSingleton(typeof(IUserAccessor), this.UserAccessorType);
        if (typeof(IHostedService).IsAssignableFrom(this.UserAccessorType)) this.Services.AddSingleton(provider => (IHostedService)provider.GetRequiredService<IUserAccessor>());

        this.Services.TryAddSingleton(typeof(IUserInfoProvider), this.UserInfoProviderType);
        if (typeof(IHostedService).IsAssignableFrom(this.UserInfoProviderType)) this.Services.AddSingleton(provider => (IHostedService)provider.GetRequiredService<IUserInfoProvider>());

        this.Services.TryAddSingleton(typeof(IPluginManager), this.PluginManagerType);
        if (typeof(IHostedService).IsAssignableFrom(this.PluginManagerType)) this.Services.AddSingleton(provider => (IHostedService)provider.GetRequiredService<IPluginManager>());

        this.Services.TryAddSingleton(typeof(IAdmissionControl), this.AdmissionControlType);
        if (typeof(IHostedService).IsAssignableFrom(this.AdmissionControlType)) this.Services.AddSingleton(provider => (IHostedService)provider.GetRequiredService<IAdmissionControl>());

        this.Services.TryAddSingleton(typeof(IVersionControl), this.VersionControlType);
        if (typeof(IHostedService).IsAssignableFrom(this.VersionControlType)) this.Services.AddSingleton(provider => (IHostedService)provider.GetRequiredService<IVersionControl>());

        this.Services.TryAddSingleton(typeof(IDatabaseProvider), this.DatabaseProviderType);
        if (typeof(IHostedService).IsAssignableFrom(this.DatabaseProviderType)) this.Services.AddSingleton(provider => (IHostedService)provider.GetRequiredService<IDatabaseProvider>());

        this.Services.TryAddSingleton(typeof(IRepository), this.RepositoryType);
        if (typeof(IHostedService).IsAssignableFrom(this.RepositoryType)) this.Services.AddSingleton(provider => (IHostedService)provider.GetRequiredService<IRepository>());

        this.Services.TryAddSingleton(typeof(IDatabaseInitializer), this.DatabaseInitializerType);
        if (typeof(IDatabaseInitializer).IsAssignableFrom(this.DatabaseInitializerType)) this.Services.AddSingleton(provider => (IHostedService)provider.GetRequiredService<IDatabaseInitializer>());

        this.Services.TryAddSingleton(provider => provider.GetRequiredService<IDatabaseProvider>().GetDatabase());

        this.Services.AddSingleton<IResourceMutator, DefaultResourceValidator>();

        this.Services.TryAddSingleton(Microsoft.Extensions.Options.Options.Create(this.Options));
    }

}