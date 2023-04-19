namespace Hylo.Providers.FileSystem;

/// <summary>
/// Defines extensions for <see cref="WatcherChangeTypes"/>
/// </summary>
public static class WatcherChangeTypesExtensions
{

    /// <summary>
    /// Converts the <see cref="WatcherChangeTypes"/> into a new <see cref="ResourceWatchEventType"/>
    /// </summary>
    /// <param name="watcherChangeTypes">The <see cref="WatcherChangeTypes"/> to convert</param>
    /// <returns>A new <see cref="ResourceWatchEventType"/></returns>
    public static ResourceWatchEventType ToResourceWatchEventType(this WatcherChangeTypes watcherChangeTypes)
    {
        return watcherChangeTypes switch
        {
            WatcherChangeTypes.Created => ResourceWatchEventType.Created,
            WatcherChangeTypes.Changed => ResourceWatchEventType.Updated,
            WatcherChangeTypes.Deleted => ResourceWatchEventType.Deleted,
            _ => throw new NotSupportedException($"The specified {nameof(WatcherChangeTypes)} '{EnumHelper.Stringify(watcherChangeTypes)}' is not supported in this context")
        };
    }

}
