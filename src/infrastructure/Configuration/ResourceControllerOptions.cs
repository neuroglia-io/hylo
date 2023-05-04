﻿namespace Hylo.Infrastructure.Configuration;

/// <summary>
/// Represents the options used to configure a <see cref="ResourceController{TResource}"/>
/// </summary>
public class ResourceControllerOptions<TResource>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Gets/sets the namespace to watch for resource events, if any
    /// </summary>
    public virtual string? ResourceNamespace { get; set; }

    /// <summary>
    /// Gets/sets a list containing the label-based selectors of controlled <see cref="IResource"/>s
    /// </summary>
    public virtual List<LabelSelector>? LabelSelectors { get; set; }

    /// <summary>
    /// Gets/sets the options used to configure the controller's reconciliation loop
    /// </summary>
    public virtual ResourceControllerReconciliationOptions Reconciliation { get; set; } = new();

}
