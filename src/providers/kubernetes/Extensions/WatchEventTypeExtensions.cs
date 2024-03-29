﻿using k8s;

namespace Hylo.Providers.Kubernetes;


/// <summary>
/// Defines extensions for <see cref="WatchEventType"/>s
/// </summary>
public static class WatchEventTypeExtensions
{

    /// <summary>
    /// Converts the watch event type to its Hylo equivalency
    /// </summary>
    /// <param name="type">The watch event type to convert</param>
    /// <returns>The converted watch event type</returns>
    public static ResourceWatchEventType ToHyloWatchEventType(this WatchEventType type)
    {
        return type switch
        {
            WatchEventType.Added => ResourceWatchEventType.Created,
            WatchEventType.Deleted => ResourceWatchEventType.Deleted,
            WatchEventType.Error => ResourceWatchEventType.Error,
            WatchEventType.Modified => ResourceWatchEventType.Updated,
            _ => throw new NotSupportedException($"The specified {nameof(WatchEventType)} '{type}' is not supported")
        };
    }

}
