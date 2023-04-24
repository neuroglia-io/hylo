using Hylo.Infrastructure.Services;
using Hylo.Resources;
using Hylo.Resources.Definitions;
using k8s;
using k8s.Autorest;
using k8s.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Hylo.Providers.Kubernetes.Services;

/// <summary>
/// Represents a <see href="https://kubernetes.io/">Kubernetes</see> implementation of an Hylo resource database
/// </summary>
public class KubernetesDatabase
    : BackgroundService, IDatabase
{

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="KubernetesDatabase"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="kubernetes">The service used to interact with Kubernetes</param>
    public KubernetesDatabase(ILoggerFactory loggerFactory, k8s.Kubernetes kubernetes)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Kubernetes = kubernetes;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to interact with Kubernetes
    /// </summary>
    protected k8s.Kubernetes Kubernetes { get; }

    /// <summary>
    /// Gets the <see cref="KubernetesDatabase"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        var seed = true;
        try
        {
            seed = await this.GetDefinitionAsync<MutatingWebhook>(this.CancellationTokenSource.Token).ConfigureAwait(false) == null;
        }
        catch { }
        if(seed)
        {
            await this.CreateResourceAsync(new MutatingWebhookDefinition(), false, this.CancellationTokenSource.Token).ConfigureAwait(false);
            await this.CreateResourceAsync(new ValidatingWebhookDefinition(), false, this.CancellationTokenSource.Token).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task<IResource> CreateResourceAsync(IResource resource, string group, string version, string plural, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (resource == null) throw new ArgumentNullException(nameof(resource));

        IResource storageResource;
        if (ResourceDefinition.ResourceGroup == group && ResourceDefinition.ResourceVersion == version && ResourceDefinition.ResourcePlural == plural)
            storageResource = (await this.Kubernetes.CreateCustomResourceDefinitionAsync(resource.ConvertTo<V1CustomResourceDefinition>(), cancellationToken: cancellationToken).ConfigureAwait(false)).ConvertTo<Resource>()!;
        else if (resource.IsNamespaced()) storageResource = (await this.Kubernetes.CreateNamespacedCustomObjectAsync(resource, group, version, @namespace, plural, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false)).ConvertTo<Resource>()!;
        else storageResource = (await this.Kubernetes.CreateClusterCustomObjectAsync(resource, group, version, plural, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false)).ConvertTo<Resource>()!;

        if(resource is IStatus status && status.Status != null)
        {
            resource.Metadata.ResourceVersion = storageResource.Metadata.ResourceVersion;
            storageResource = (await this.ReplaceSubResourceAsync(resource, group, version, plural, storageResource.GetName(), "status", @namespace, dryRun, cancellationToken).ConfigureAwait(false)).ConvertTo<Resource>()!;
        }

        return storageResource;
    }

    /// <inheritdoc/>
    public async Task<IResource?> GetResourceAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(name));

        try
        {
            object? resource;
            if (ResourceDefinition.ResourceGroup == group && ResourceDefinition.ResourceVersion == version && ResourceDefinition.ResourcePlural == plural)
            {
                if (name == NamespaceDefinition.ResourcePlural) return NamespaceDefinition.Instance;
                resource = await this.Kubernetes.ReadCustomResourceDefinitionAsync(name, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else if (string.IsNullOrWhiteSpace(@namespace)) resource = await this.Kubernetes.CustomObjects.GetClusterCustomObjectAsync(group, version, plural, name, cancellationToken).ConfigureAwait(false);
            else resource = await this.Kubernetes.CustomObjects.GetNamespacedCustomObjectAsync(group, version, @namespace, plural, name, cancellationToken).ConfigureAwait(false);

            return resource.ConvertTo<Resource>();
        }
        catch (HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound) 
        {
            return null;
        }
        
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<IResource> GetResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        object result;
        if (ResourceDefinition.ResourceGroup == group && ResourceDefinition.ResourceVersion == version && ResourceDefinition.ResourcePlural == plural)
            result = await this.Kubernetes.ListCustomResourceDefinitionAsync(labelSelector: labelSelectors.ToExpression(), cancellationToken: cancellationToken).ConfigureAwait(false);
        else if (string.IsNullOrWhiteSpace(@namespace)) result = await this.Kubernetes.CustomObjects.ListClusterCustomObjectAsync(group, version, plural, labelSelector: labelSelectors.ToExpression(), cancellationToken: cancellationToken).ConfigureAwait(false);
        else result = await this.Kubernetes.CustomObjects.ListNamespacedCustomObjectAsync(group, version, @namespace, plural, labelSelector: labelSelectors.ToExpression(), cancellationToken: cancellationToken).ConfigureAwait(false);
        
        await foreach (var resource in result.ConvertTo<Collection<Resource>>()!.Items!.ToAsyncEnumerable())
        {
            yield return resource;
        }
    }

    /// <inheritdoc/>
    public async Task<ICollection> ListResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        object result;
        if (ResourceDefinition.ResourceGroup == group && ResourceDefinition.ResourceVersion == version && ResourceDefinition.ResourcePlural == plural)
            result = await this.Kubernetes.ListCustomResourceDefinitionAsync(continueParameter: continuationToken, labelSelector: labelSelectors.ToExpression(), cancellationToken: cancellationToken).ConfigureAwait(false);
        else if (string.IsNullOrWhiteSpace(@namespace)) result = await this.Kubernetes.CustomObjects.ListClusterCustomObjectAsync(group, version, plural, continueParameter: continuationToken, labelSelector: labelSelectors.ToExpression(), cancellationToken: cancellationToken).ConfigureAwait(false);
        else result = await this.Kubernetes.CustomObjects.ListNamespacedCustomObjectAsync(group, version, @namespace, plural, continueParameter: continuationToken, labelSelector: labelSelectors.ToExpression(), cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return result.ConvertTo<Collection>()!;
    }

    /// <inheritdoc/>
    public async Task<IResourceWatch> WatchResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        HttpOperationResponse<object> response;

        if (string.IsNullOrWhiteSpace(@namespace)) response = await this.Kubernetes.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync(group, version, plural, labelSelector: labelSelectors.ToExpression(), watch: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        else response = await this.Kubernetes.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync(group, version, @namespace, plural, labelSelector: labelSelectors.ToExpression(), watch: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        
        var subject = new Subject<IResourceWatchEvent>();
        var watch = response.Watch((WatchEventType watchEventType, object resource) => subject.OnNext(new ResourceWatchEvent(watchEventType.ToHyloWatchEventType(), resource.ConvertTo<Resource>()!)));
        return new ResourceWatch(Observable.Using(() => watch, _ => subject), true);
    }

    /// <inheritdoc/>
    public async Task<IResource> PatchResourceAsync(Patch patch, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (patch == null) throw new ArgumentNullException(nameof(patch));
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        object resource;
        if(string.IsNullOrWhiteSpace(@namespace)) resource = await this.Kubernetes.PatchClusterCustomObjectAsync(patch.ToV1Patch(), group, version, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);
        else resource = await this.Kubernetes.PatchNamespacedCustomObjectAsync(patch.ToV1Patch(), group, version, @namespace, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);

        return resource.ConvertTo<Resource>()!;
    }

    /// <inheritdoc/>
    public async Task<IResource> ReplaceResourceAsync(IResource resource, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        object storageResource;
        if (resource.IsNamespaced()) storageResource = await this.Kubernetes.ReplaceNamespacedCustomObjectAsync(resource.ConvertTo<Resource>(), group, version, @namespace, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);
        else storageResource = await this.Kubernetes.ReplaceClusterCustomObjectAsync(resource.ConvertTo<Resource>(), group, version, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);

        return storageResource.ConvertTo<Resource>()!;
    }

    /// <inheritdoc/>
    public async Task<IResource> PatchSubResourceAsync(Patch patch, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (patch == null) throw new ArgumentNullException(nameof(patch));
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(subResource)) throw new ArgumentNullException(nameof(subResource));
        if (subResource != "status") throw new HyloException(ProblemDetails.UnsupportedSubResource(new SubResourceReference(new(group, version, plural), name, subResource, @namespace)));

        object resource;
        var k8sPatch = patch.ToV1Patch();
        if (string.IsNullOrWhiteSpace(@namespace)) resource = await this.Kubernetes.PatchClusterCustomObjectStatusAsync(k8sPatch, group, version, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);
        else resource = await this.Kubernetes.PatchNamespacedCustomObjectStatusAsync(k8sPatch, group, version, @namespace, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);

        return resource.ConvertTo<Resource>()!;
    }

    /// <inheritdoc/>
    public async Task<IResource> ReplaceSubResourceAsync(IResource resource, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(subResource)) throw new ArgumentNullException(nameof(subResource));
        if (subResource != "status") throw new HyloException(ProblemDetails.UnsupportedSubResource(new SubResourceReference(new(group, version, plural), name, subResource, @namespace)));

        object storageResource;
        if (resource.IsNamespaced()) storageResource = await this.Kubernetes.ReplaceNamespacedCustomObjectStatusAsync(resource, group, version, @namespace, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);
        else storageResource = await this.Kubernetes.ReplaceClusterCustomObjectStatusAsync(resource, group, version, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);

        return storageResource.ConvertTo<Resource>()!;
    }

    /// <inheritdoc/>
    public async Task<IResource> DeleteResourceAsync(string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resource = string.IsNullOrWhiteSpace(@namespace) ?
            await this.Kubernetes.CustomObjects.GetClusterCustomObjectAsync(group, version, plural, name, cancellationToken).ConfigureAwait(false)
            : await this.Kubernetes.CustomObjects.GetNamespacedCustomObjectAsync(group, version, @namespace, plural, name, cancellationToken).ConfigureAwait(false);
        if (ResourceDefinition.ResourceGroup == group && ResourceDefinition.ResourceVersion == version && ResourceDefinition.ResourcePlural == plural)
        {
            await this.Kubernetes.DeleteCustomResourceDefinitionAsync(name, cancellationToken: cancellationToken).ConfigureAwait(false);
            return resource.ConvertTo<Resource>()!;
        }

        object? updatedResource;
        if (string.IsNullOrWhiteSpace(@namespace)) updatedResource = await this.Kubernetes.CustomObjects.DeleteClusterCustomObjectAsync(group, version, plural, name, cancellationToken: cancellationToken).ConfigureAwait(false);
        else updatedResource = await this.Kubernetes.CustomObjects.DeleteNamespacedCustomObjectAsync(group, version, @namespace, plural, name, cancellationToken: cancellationToken).ConfigureAwait(false);

        return updatedResource.ConvertTo<Resource>()!;
    }

    /// <summary>
    /// Disposes of the <see cref="KubernetesDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="KubernetesDatabase"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (this._disposed || !disposing) return ValueTask.CompletedTask;
        this.CancellationTokenSource.Dispose();
        this._disposed = true;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="KubernetesDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="KubernetesDatabase"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this.CancellationTokenSource?.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

}
