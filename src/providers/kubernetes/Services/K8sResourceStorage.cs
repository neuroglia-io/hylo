using Hylo.Infrastructure.Services;
using Hylo.Resources.Definitions;
using k8s;
using k8s.Autorest;
using k8s.Models;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Hylo.Providers.Kubernetes.Services;

/// <summary>
/// Represents the <see href="https://kubernetes.io/">Kubernetes</see> implementation of the <see cref="IResourceStorage"/> interface
/// </summary>
public class K8sResourceStorage
    : IResourceStorage
{

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="K8sResourceStorage"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="kubernetes">The service used to interact with Kubernetes</param>
    public K8sResourceStorage(ILoggerFactory loggerFactory, k8s.Kubernetes kubernetes)
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

    /// <inheritdoc/>
    public virtual async Task<IResource> WriteAsync(IResource resource, string group, string version, string plural, string? @namespace = null, string? subResource = null, bool ifNotExists = false, CancellationToken cancellationToken = default)
    {
        try
        {

            if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
            if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            object storedResource;
            if (ifNotExists)
            {
                if (ResourceDefinition.ResourceGroup == group && ResourceDefinition.ResourceVersion == version && ResourceDefinition.ResourcePlural == plural)
                    storedResource = await this.Kubernetes.CreateCustomResourceDefinitionAsync(resource.ConvertTo<V1CustomResourceDefinition>(), cancellationToken: cancellationToken).ConfigureAwait(false);
                else if (string.IsNullOrWhiteSpace(@namespace)) storedResource = await this.Kubernetes.CustomObjects.CreateClusterCustomObjectAsync(resource, group, version, plural, cancellationToken: cancellationToken).ConfigureAwait(false);
                else storedResource = await this.Kubernetes.CustomObjects.CreateNamespacedCustomObjectAsync(resource, group, version, @namespace, plural, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (ResourceDefinition.ResourceGroup == group && ResourceDefinition.ResourceVersion == version && ResourceDefinition.ResourcePlural == plural)
                    storedResource = await this.Kubernetes.ReplaceCustomResourceDefinitionAsync(resource.ConvertTo<V1CustomResourceDefinition>(), resource.GetName(), cancellationToken: cancellationToken).ConfigureAwait(false);
                else if (string.IsNullOrWhiteSpace(@namespace)) storedResource = await this.Kubernetes.CustomObjects.ReplaceClusterCustomObjectAsync(resource, group, version, plural, resource.GetName(), cancellationToken: cancellationToken).ConfigureAwait(false);
                else storedResource = await this.Kubernetes.CustomObjects.ReplaceNamespacedCustomObjectAsync(resource, group, version, @namespace, plural, resource.GetName(), cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            return storedResource.ConvertTo<Resource>()!;
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection> ReadAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
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
    public virtual async IAsyncEnumerable<IResource> ReadAllAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
    public virtual async Task<IResourceWatch> WatchAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
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
    public virtual async Task<IResource?> ReadOneAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(name));

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

    /// <inheritdoc/>
    public virtual async Task<IResource> DeleteAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
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
    /// Disposes of the <see cref="K8sResourceStorage"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="K8sResourceStorage"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (this._disposed || !disposing) return ValueTask.CompletedTask;

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
    /// Disposes of the <see cref="K8sResourceStorage"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="K8sResourceStorage"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed || !disposing) return;

        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

}
