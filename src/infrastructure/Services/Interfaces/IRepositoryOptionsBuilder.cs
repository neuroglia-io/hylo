﻿using Microsoft.Extensions.Configuration;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to build an <see cref="IRepository"/>
/// </summary>
public interface IRepositoryOptionsBuilder
{

    /// <summary>
    /// Gets the current <see cref="IConfiguration"/>
    /// </summary>
    IConfiguration Configuration { get; }

    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> to configure
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/>s to use the specified <see cref="IResourceDefinition"/> kind
    /// </summary>
    /// <remarks>The same outcome can be achieved by setting related static fields on the <see cref="IResourceDefinition"/> class</remarks>
    /// <param name="group">The API group <see cref="IResourceDefinition"/>s to use belong to</param>
    /// <param name="version">The version of the API <see cref="IResourceDefinition"/>s to use belong to</param>
    /// <param name="plural">The plural name of <see cref="IResourceDefinition"/>s used by the <see cref="IRepository"/> to configure</param>
    /// <param name="kind">The kind of <see cref="IResourceDefinition"/>s used by the <see cref="IRepository"/> to configure</param>
    /// <returns>The configured <see cref="IRepository"/></returns>
    IRepositoryOptionsBuilder UseDefinitionsKind(string group, string version, string plural, string kind);

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/>s to use the specified <see cref="Namespace"/> kind
    /// </summary>
    /// <remarks>The same outcome can be achieved by setting related static fields on the <see cref="Namespace"/> class</remarks>
    /// <param name="group">The API group namespaces to use belong to</param>
    /// <param name="version">The version of the API namespaces to use belong to</param>
    /// <param name="plural">The plural name of namespaces used by the <see cref="IRepository"/> to configure</param>
    /// <param name="kind">The kind of namespaces used by the <see cref="IRepository"/> to configure</param>
    /// <returns>The configured <see cref="IRepository"/></returns>
    IRepositoryOptionsBuilder UseNamespacesKind(string group, string version, string plural, string kind);

    /// <summary>
    /// Configures the name of the default namespace used by the <see cref="IRepository"/> to configure
    /// </summary>
    /// <param name="name">The default namespace's name</param>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    IRepositoryOptionsBuilder WithDefaultNamespace(string name);

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to used the specified type of <see cref="IPluginManager"/>
    /// </summary>
    /// <typeparam name="TManager">The type of <see cref="IPluginManager"/> to use</typeparam>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    IRepositoryOptionsBuilder UsePluginManager<TManager>()
        where TManager : class, IPluginManager;

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to used the specified type of <see cref="IAdmissionControl"/>
    /// </summary>
    /// <typeparam name="TControl">The type of <see cref="IAdmissionControl"/> to use</typeparam>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    IRepositoryOptionsBuilder UseAdmissionControl<TControl>()
        where TControl : class, IAdmissionControl;

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to used the specified type of <see cref="IVersionControl"/>
    /// </summary>
    /// <typeparam name="TControl">The type of <see cref="IVersionControl"/> to use</typeparam>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    IRepositoryOptionsBuilder UseVersionControl<TControl>()
        where TControl : class, IVersionControl;

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to used the specified type of <see cref="IUserAccessor"/>
    /// </summary>
    /// <typeparam name="TAccessor">The type of <see cref="IUserAccessor"/> to use</typeparam>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    IRepositoryOptionsBuilder UseUserAccessor<TAccessor>()
        where TAccessor : class, IUserAccessor;

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to used the specified type of <see cref="IDatabaseInitializer"/>
    /// </summary>
    /// <typeparam name="TInitializer">The type of <see cref="IDatabaseInitializer"/> to use</typeparam>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    IRepositoryOptionsBuilder UseDatabaseInitializer<TInitializer>()
        where TInitializer : class, IDatabaseInitializer;

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to used the specified type of <see cref="IUserInfoProvider"/>
    /// </summary>
    /// <typeparam name="TProvider">The type of <see cref="IUserInfoProvider"/> to use</typeparam>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    IRepositoryOptionsBuilder UseUserInfoProvider<TProvider>()
        where TProvider : class, IUserInfoProvider;

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to used the specified type of <see cref="IDatabaseProvider"/>
    /// </summary>
    /// <typeparam name="TProvider">The type of <see cref="IDatabaseProvider"/> to use</typeparam>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    IRepositoryOptionsBuilder UseDatabaseProvider<TProvider>()
        where TProvider : class, IDatabaseProvider;

    /// <summary>
    /// Configures the <see cref="IRepositoryOptionsBuilder"/> to used the specified type of <see cref="IRepository"/>
    /// </summary>
    /// <typeparam name="TRepository">The type of <see cref="IRepository"/> to use</typeparam>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    IRepositoryOptionsBuilder UseRepository<TRepository>()
        where TRepository : class, IRepository;

    /// <summary>
    /// Builds the configured <see cref="IRepository"/>
    /// </summary>
    void Build();

}