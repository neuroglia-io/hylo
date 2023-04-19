using Hylo.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IResourceRepository"/> interface
/// </summary>
public class ResourceRepository
    : IHostedService, IResourceRepository
{
    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="ResourceRepository"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="admissionControl">The service used to control admission of resource operations</param>
    /// <param name="versionControl">The service used to control versioning of admitted resources</param>
    /// <param name="storage">The service used to read and write the resource</param>
    /// <param name="options">The service used to access the current <see cref="ResourceRepositoryOptions"/></param>
    public ResourceRepository(ILoggerFactory loggerFactory, IAdmissionControl admissionControl, IVersionControl versionControl, IResourceStorage storage, IOptions<ResourceRepositoryOptions> options)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.AdmissionControl = admissionControl;
        this.VersionControl = versionControl;
        this.Storage = storage;
        this.Options = options.Value;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to control admission of resource operations
    /// </summary>
    protected IAdmissionControl AdmissionControl { get; }

    /// <summary>
    /// Gets the service used to control versioning of admitted resources
    /// </summary>
    protected IVersionControl VersionControl { get; }

    /// <summary>
    /// Gets the service used to read and write the resource
    /// </summary>
    protected IResourceStorage Storage { get; }

    /// <summary>
    /// Gets the current <see cref="ResourceRepositoryOptions"/>
    /// </summary>
    protected ResourceRepositoryOptions Options { get; }

    /// <summary>
    /// Gets the <see cref="ResourceRepository"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource? CancellationTokenSource { get; private set; }

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        return this.SeedAsync(this.CancellationTokenSource!.Token);
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.CancellationTokenSource?.Cancel();
        this.CancellationTokenSource?.Dispose();
        this.CancellationTokenSource = null;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Seeds the <see cref="IResourceRepository"/>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach(var definition in this.Options.WellKnownDefinitions)
        {
            try
            {
                await this.AddAsync(definition.ConvertTo<ResourceDefinition>()!, false, cancellationToken).ConfigureAwait(false);
            }
            catch { }
        }

        foreach(var resource in this.Options.WellKnownResources)
        {
            try
            {
                await this.AddAsync(resource, resource.GetGroup(), resource.GetVersion(), resource.Definition.Plural, resource.GetNamespace(), false, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            { 
            
            }
        }
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> AddAsync(IResource resource, string group, string version, string plural, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        IResource storageResource;
        if (resource.IsResourceDefinition() || resource.IsNamespace())
        {
            storageResource = resource;
        }
        else
        {
            var resourceReference = new ResourceReference(new(group, version, plural), resource.GetName(), @namespace);
            var resourceDefinition = await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceDefinitionNotFound(resourceReference));
            var result = await this.AdmissionControl.ReviewAsync(ResourceOperation.Create, resourceDefinition, resourceReference, null, resource, null, dryRun, cancellationToken).ConfigureAwait(false);
            if (!result.Allowed) throw new HyloException(ProblemDetails.ResourceAdmissionFailed(ResourceOperation.Create, resourceReference, result.Errors?.ToArray()!));
            storageResource = result.Resource!;

            var storageVersion = resourceDefinition.GetStorageVersion();
            if (resource.ApiVersion != storageVersion.Name) storageResource = await this.VersionControl.ConvertToStorageVersionAsync(new(resourceReference, resourceDefinition, storageResource), cancellationToken).ConfigureAwait(false);
        }

        if (!dryRun) storageResource = await this.Storage.WriteAsync(storageResource, group, version, plural, @namespace, null, true, cancellationToken).ConfigureAwait(false);
        return storageResource;
    }

    /// <inheritdoc/>
    public virtual Task<IResource?> GetAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        return this.Storage.ReadOneAsync(group, version, plural, name, @namespace, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<IResource> GetAllAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        return this.Storage.ReadAllAsync(group, version, plural, @namespace, labelSelectors, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<ICollection> ListAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        return this.Storage.ReadAsync(group, version, plural, @namespace, labelSelectors, maxResults, continuationToken, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<IResourceWatch> WatchAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        return this.Storage.WatchAsync(group, version, plural, @namespace, labelSelectors, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> UpdateAsync(IResource resource, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var originalResource = await this.GetAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var resourceDefinition = (await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false))!;

        var result = await this.AdmissionControl.ReviewAsync(ResourceOperation.Update, resourceDefinition, resourceReference, null, resource, originalResource, dryRun, cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) throw new HyloException(ProblemDetails.ResourceAdmissionFailed(ResourceOperation.Update, resourceReference, result.Errors?.ToArray()!));

        var storageResource = result.Resource!;
        var storageVersion = resourceDefinition.GetStorageVersion();
        if (storageResource.ApiVersion != storageVersion.Name) storageResource = await this.VersionControl.ConvertToStorageVersionAsync(new(resourceReference, resourceDefinition, storageResource), cancellationToken).ConfigureAwait(false);

        if (!dryRun) storageResource = await this.Storage.WriteAsync(storageResource, group, version, plural, @namespace, null, false, cancellationToken).ConfigureAwait(false);

        return storageResource;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchAsync(Patch patch, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (patch == null) throw new ArgumentNullException(nameof(patch));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var originalResource = await this.GetAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var resourceDefinition = (await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false))!;

        var patchedResource = patch.ApplyTo(originalResource.ConvertTo<Resource>());

        var result = await this.AdmissionControl.ReviewAsync(ResourceOperation.Update, resourceDefinition, resourceReference, null, patchedResource, originalResource, dryRun, cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) throw new HyloException(ProblemDetails.ResourceAdmissionFailed(ResourceOperation.Update, resourceReference, result.Errors?.ToArray()!));

        var storageResource = result.Resource!;
        var storageVersion = resourceDefinition.GetStorageVersion();
        if (storageResource.ApiVersion != storageVersion.Name) storageResource = await this.VersionControl.ConvertToStorageVersionAsync(new(resourceReference, resourceDefinition, storageResource), cancellationToken).ConfigureAwait(false);

        if (!dryRun) storageResource = await this.Storage.WriteAsync(storageResource, group, version, plural, @namespace, null, false, cancellationToken).ConfigureAwait(false);

        return storageResource;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> UpdateStatusAsync(IResource resource, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var originalResource = await this.GetAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var resourceDefinition = (await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false))!;

        var result = await this.AdmissionControl.ReviewAsync(ResourceOperation.Update, resourceDefinition, resourceReference, null, resource, originalResource, dryRun, cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) throw new HyloException(ProblemDetails.ResourceAdmissionFailed(ResourceOperation.Update, resourceReference, result.Errors?.ToArray()!));

        var storageResource = result.Resource!;
        var storageVersion = resourceDefinition.GetStorageVersion();
        if (storageResource.ApiVersion != storageVersion.Name) storageResource = await this.VersionControl.ConvertToStorageVersionAsync(new(resourceReference, resourceDefinition, storageResource), cancellationToken).ConfigureAwait(false);

        if (!dryRun) storageResource = await this.Storage.WriteAsync(storageResource, group, version, plural, @namespace, "status", false, cancellationToken).ConfigureAwait(false);

        return storageResource;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchStatusAsync(Patch patch, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (patch == null) throw new ArgumentNullException(nameof(patch));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var originalResource = await this.GetAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var resourceDefinition = (await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false))!;

        var patchedResource = patch.ApplyTo(originalResource.ConvertTo<Resource>());

        var result = await this.AdmissionControl.ReviewAsync(ResourceOperation.Update, resourceDefinition, resourceReference, null, patchedResource, originalResource, dryRun, cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) throw new HyloException(ProblemDetails.ResourceAdmissionFailed(ResourceOperation.Update, resourceReference, result.Errors?.ToArray()!));

        var storageResource = result.Resource!;
        var storageVersion = resourceDefinition.GetStorageVersion();
        if (storageResource.ApiVersion != storageVersion.Name) storageResource = await this.VersionControl.ConvertToStorageVersionAsync(new(resourceReference, resourceDefinition, storageResource), cancellationToken).ConfigureAwait(false);

        if (!dryRun) storageResource = await this.Storage.WriteAsync(storageResource, group, version, plural, @namespace, "status", false, cancellationToken).ConfigureAwait(false);

        return storageResource;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> RemoveAsync(string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var originalResource = await this.GetAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var resourceDefinition = (await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false))!;

        var result = await this.AdmissionControl.ReviewAsync(ResourceOperation.Delete, resourceDefinition, resourceReference, null, null, originalResource, dryRun, cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) throw new HyloException(ProblemDetails.ResourceAdmissionFailed(ResourceOperation.Delete, resourceReference, result.Errors?.ToArray()!));

        var storageResource = result.Resource!;
        if (!dryRun) await this.Storage.DeleteAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false);

        return storageResource;
    }

    /// <summary>
    /// Disposes of the <see cref="IResourceRepository"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not to dispose of the <see cref="IResourceRepository"/></param>
    /// <returns>A new <see cref="ValueTask"/></returns>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this.CancellationTokenSource?.Dispose();
            }
            this._disposed = true;
        }
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(disposing: true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="IResourceRepository"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not to dispose of the <see cref="IResourceRepository"/></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this.CancellationTokenSource?.Dispose();
            }
            this._disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}