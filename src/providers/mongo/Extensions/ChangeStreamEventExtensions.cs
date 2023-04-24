using MongoDB.Driver;

namespace Hylo.Providers.Mongo;

/// <summary>
/// Defines extensions for <see cref="ChangeStreamOperationType"/>s
/// </summary>
public static class ChangeStreamEventExtensions
{

    /// <summary>
    /// Determines whether or not the <see cref="ChangeStreamOperationType"/> describes a potential <see cref="ResourceWatchEventType"/>
    /// </summary>
    /// <param name="changeStreamOperationType">The <see cref="ChangeStreamOperationType"/> to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="ChangeStreamOperationType"/> describes a potential <see cref="ResourceWatchEventType"/></returns>
    public static bool IsResourceWatchEventType(this ChangeStreamOperationType changeStreamOperationType)
    {
        return changeStreamOperationType switch
        {
            ChangeStreamOperationType.Create or ChangeStreamOperationType.Insert or ChangeStreamOperationType.Update or ChangeStreamOperationType.Modify or ChangeStreamOperationType.Replace or ChangeStreamOperationType.Delete => true,
            _ => false
        };
    }

    /// <summary>
    /// Converts the specified <see cref="ChangeStreamOperationType"/> into a new <see cref="ResourceWatchEventType"/>
    /// </summary>
    /// <param name="changeStreamOperationType">The <see cref="ChangeStreamOperationType"/> to convert</param>
    /// <returns>The converted <see cref="ResourceWatchEventType"/></returns>
    public static ResourceWatchEventType ToResourceWatchEventType(this ChangeStreamOperationType changeStreamOperationType)
    {
        return changeStreamOperationType switch
        {
            ChangeStreamOperationType.Create or ChangeStreamOperationType.Insert => ResourceWatchEventType.Created,
            ChangeStreamOperationType.Update or ChangeStreamOperationType.Modify or ChangeStreamOperationType.Replace => ResourceWatchEventType.Updated,
            ChangeStreamOperationType.Delete => ResourceWatchEventType.Deleted,
            _ => throw new NotSupportedException($"The specified {nameof(ChangeStreamOperationType)} '{changeStreamOperationType}' is not supported")
        };
    }

}