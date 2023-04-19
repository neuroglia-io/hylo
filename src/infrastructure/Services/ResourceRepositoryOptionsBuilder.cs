﻿using Hylo.Infrastructure.Configuration;
using Hylo.Resources.Definitions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IResourceRepositoryOptionsBuilder"/> interface
/// </summary>
public class ResourceRepositoryOptionsBuilder
    : IResourceRepositoryOptionsBuilder
{

    /// <summary>
    /// Initializes a new <see cref="ResourceRepositoryOptionsBuilder"/>
    /// </summary>
    /// <param name="configuration">The current <see cref="IConfiguration"/></param>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    public ResourceRepositoryOptionsBuilder(IConfiguration configuration, IServiceCollection services)
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
    /// Gets/sets the type of <see cref="IResourceStorageProvider"/> to use
    /// </summary>
    protected Type StorageProviderType { get; set; } = typeof(PluginResourceStorageProvider);

    /// <summary>
    /// Gets/sets the type of <see cref="IResourceRepository"/> to use
    /// </summary>
    protected Type RepositoryType { get; set; } = typeof(ResourceRepository);

    /// <summary>
    /// Gets the <see cref="ResourceRepositoryOptions"/> to configure
    /// </summary>
    protected ResourceRepositoryOptions Options { get; } = new();

    /// <inheritdoc/>
    public virtual IResourceRepositoryOptionsBuilder UseDefinitionsKind(string group, string version, string plural, string kind)
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
    public virtual IResourceRepositoryOptionsBuilder UseNamespacesKind(string group, string version, string plural, string kind)
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
    public virtual IResourceRepositoryOptionsBuilder WithDefaultNamespace(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        ObjectNamingConvention.Current.EnsureIsValidResourceName(name);

        Namespace.DefaultNamespaceName = name;

        return this;
    }

    /// <inheritdoc/>
    public virtual IResourceRepositoryOptionsBuilder WithDefinition(IResourceDefinition definition)
    {
        if (definition == null) throw new ArgumentNullException(nameof(definition));

        this.Options.WellKnownDefinitions.Add(definition);

        return this;
    }

    /// <inheritdoc/>
    public virtual IResourceRepositoryOptionsBuilder WithResource(IResource resource)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));

        this.Options.WellKnownResources.Add(resource);

        return this;
    }

    IResourceRepositoryOptionsBuilder IResourceRepositoryOptionsBuilder.UseUserAccessor<TAccessor>()
    {
        this.UserAccessorType = typeof(TAccessor);
        return this;
    }

    IResourceRepositoryOptionsBuilder IResourceRepositoryOptionsBuilder.UseUserInfoProvider<TProvider>()
    {
        this.UserInfoProviderType = typeof(TProvider);
        return this;
    }

    IResourceRepositoryOptionsBuilder IResourceRepositoryOptionsBuilder.UsePluginManager<TManager>()
    {
        this.PluginManagerType = typeof(TManager);
        return this;
    }

    IResourceRepositoryOptionsBuilder IResourceRepositoryOptionsBuilder.UseAdmissionControl<TControl>()
    {
        this.AdmissionControlType = typeof(TControl);
        return this;
    }

    IResourceRepositoryOptionsBuilder IResourceRepositoryOptionsBuilder.UseVersionControl<TControl>()
    {
        this.VersionControlType = typeof(TControl);
        return this;
    }

    IResourceRepositoryOptionsBuilder IResourceRepositoryOptionsBuilder.UseStorageProvider<TProvider>()
    {
        this.StorageProviderType = typeof(TProvider);
        return this;
    }

    IResourceRepositoryOptionsBuilder IResourceRepositoryOptionsBuilder.UseRepository<TRepository>()
    {
        this.RepositoryType = typeof(TRepository);
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

        this.Services.TryAddSingleton(typeof(IResourceStorageProvider), this.StorageProviderType);
        if (typeof(IHostedService).IsAssignableFrom(this.StorageProviderType)) this.Services.AddSingleton(provider => (IHostedService)provider.GetRequiredService<IResourceStorageProvider>());

        this.Services.TryAddSingleton(typeof(IResourceRepository), this.RepositoryType);
        if (typeof(IHostedService).IsAssignableFrom(this.RepositoryType)) this.Services.AddSingleton(provider => (IHostedService)provider.GetRequiredService<IResourceRepository>());

        this.Services.TryAddSingleton(provider => provider.GetRequiredService<IResourceStorageProvider>().GetResourceStorage());

        this.Services.TryAddSingleton(Microsoft.Extensions.Options.Options.Create(this.Options));
    }

}
