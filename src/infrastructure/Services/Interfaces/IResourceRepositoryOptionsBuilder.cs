using Microsoft.Extensions.Configuration;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to build an <see cref="IResourceRepository"/>
/// </summary>
public interface IResourceRepositoryOptionsBuilder
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
    /// Configures the <see cref="IResourceRepositoryOptionsBuilder"/>s to use the specified <see cref="IResourceDefinition"/> kind
    /// </summary>
    /// <remarks>The same outcome can be achieved by setting related static fields on the <see cref="IResourceDefinition"/> class</remarks>
    /// <param name="group">The API group <see cref="IResourceDefinition"/>s to use belong to</param>
    /// <param name="version">The version of the API <see cref="IResourceDefinition"/>s to use belong to</param>
    /// <param name="plural">The plural name of <see cref="IResourceDefinition"/>s used by the <see cref="IResourceRepository"/> to configure</param>
    /// <param name="kind">The kind of <see cref="IResourceDefinition"/>s used by the <see cref="IResourceRepository"/> to configure</param>
    /// <returns>The configured <see cref="IResourceRepository"/></returns>
    IResourceRepositoryOptionsBuilder UseDefinitionsKind(string group, string version, string plural, string kind);

    /// <summary>
    /// Configures the <see cref="IResourceRepositoryOptionsBuilder"/>s to use the specified <see cref="Namespace"/> kind
    /// </summary>
    /// <remarks>The same outcome can be achieved by setting related static fields on the <see cref="Namespace"/> class</remarks>
    /// <param name="group">The API group namespaces to use belong to</param>
    /// <param name="version">The version of the API namespaces to use belong to</param>
    /// <param name="plural">The plural name of namespaces used by the <see cref="IResourceRepository"/> to configure</param>
    /// <param name="kind">The kind of namespaces used by the <see cref="IResourceRepository"/> to configure</param>
    /// <returns>The configured <see cref="IResourceRepository"/></returns>
    IResourceRepositoryOptionsBuilder UseNamespacesKind(string group, string version, string plural, string kind);

    /// <summary>
    /// Configures the name of the default namespace used by the <see cref="IResourceRepository"/> to configure
    /// </summary>
    /// <param name="name">The default namespace's name</param>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    IResourceRepositoryOptionsBuilder WithDefaultNamespace(string name);

    /// <summary>
    /// Seeds the <see cref="IResourceRepository"/> to configure with the specified well-known <see cref="IResourceDefinition"/>
    /// </summary>
    /// <param name="definition">The <see cref="IResourceDefinition"/> to use</param>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    IResourceRepositoryOptionsBuilder WithDefinition(IResourceDefinition definition);

    /// <summary>
    /// Seeds the <see cref="IResourceRepository"/> to configure with the specified well-known <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to seed the <see cref="IResourceRepository"/> with</param>
    /// <returns>The configured <see cref="IResourceRepository"/></returns>
    IResourceRepositoryOptionsBuilder WithResource(IResource resource);

    /// <summary>
    /// Configures the <see cref="IResourceRepositoryOptionsBuilder"/> to used the specified type of <see cref="IPluginManager"/>
    /// </summary>
    /// <typeparam name="TManager">The type of <see cref="IPluginManager"/> to use</typeparam>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    IResourceRepositoryOptionsBuilder UsePluginManager<TManager>()
        where TManager : class, IPluginManager;

    /// <summary>
    /// Configures the <see cref="IResourceRepositoryOptionsBuilder"/> to used the specified type of <see cref="IAdmissionControl"/>
    /// </summary>
    /// <typeparam name="TControl">The type of <see cref="IAdmissionControl"/> to use</typeparam>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    IResourceRepositoryOptionsBuilder UseAdmissionControl<TControl>()
        where TControl : class, IAdmissionControl;

    /// <summary>
    /// Configures the <see cref="IResourceRepositoryOptionsBuilder"/> to used the specified type of <see cref="IVersionControl"/>
    /// </summary>
    /// <typeparam name="TControl">The type of <see cref="IVersionControl"/> to use</typeparam>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    IResourceRepositoryOptionsBuilder UseVersionControl<TControl>()
        where TControl : class, IVersionControl;

    /// <summary>
    /// Configures the <see cref="IResourceRepositoryOptionsBuilder"/> to used the specified type of <see cref="IUserAccessor"/>
    /// </summary>
    /// <typeparam name="TAccessor">The type of <see cref="IUserAccessor"/> to use</typeparam>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    IResourceRepositoryOptionsBuilder UseUserAccessor<TAccessor>()
        where TAccessor : class, IUserAccessor;

    /// <summary>
    /// Configures the <see cref="IResourceRepositoryOptionsBuilder"/> to used the specified type of <see cref="IUserInfoProvider"/>
    /// </summary>
    /// <typeparam name="TProvider">The type of <see cref="IUserInfoProvider"/> to use</typeparam>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    IResourceRepositoryOptionsBuilder UseUserInfoProvider<TProvider>()
        where TProvider : class, IUserInfoProvider;

    /// <summary>
    /// Configures the <see cref="IResourceRepositoryOptionsBuilder"/> to used the specified type of <see cref="IResourceStorageProvider"/>
    /// </summary>
    /// <typeparam name="TProvider">The type of <see cref="IResourceStorageProvider"/> to use</typeparam>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    IResourceRepositoryOptionsBuilder UseStorageProvider<TProvider>()
        where TProvider : class, IResourceStorageProvider;

    /// <summary>
    /// Configures the <see cref="IResourceRepositoryOptionsBuilder"/> to used the specified type of <see cref="IResourceRepository"/>
    /// </summary>
    /// <typeparam name="TRepository">The type of <see cref="IResourceRepository"/> to use</typeparam>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    IResourceRepositoryOptionsBuilder UseRepository<TRepository>()
        where TRepository : class, IResourceRepository;

    /// <summary>
    /// Builds the configured <see cref="IResourceRepository"/>
    /// </summary>
    void Build();

}
