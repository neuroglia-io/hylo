using Hylo.Api.Core.Data.Models;

namespace Hylo.Api;

/// <summary>
/// Defines extensions for <see cref="V1ResourceEvent"/>s
/// </summary>
public static class V1ResourceEventExtensions
{

    /// <summary>
    /// Converts the <see cref="V1ResourceEvent"/> into a generic, resource-specific <see cref="V1ResourceEvent{TResource}"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="V1Resource"/> to create the new <see cref="V1ResourceEvent{TResource}"/> for</typeparam>
    /// <param name="e">The <see cref="V1ResourceEvent"/> to convert</param>
    /// <returns>A new <see cref="V1ResourceEvent{TResource}"/> for the specified <see cref="V1Resource"/> type</returns>
    public static V1ResourceEvent<TResource>? ForResourceOfType<TResource>(this V1ResourceEvent e)
        where TResource : V1Resource
    {
        if(e == null) throw new ArgumentNullException(nameof(e));
        if (e is V1ResourceEvent<TResource> generic) return generic;
        var resource = e.Resource as TResource;
        if(resource == null)
        {
            if (resource is JsonObject jsonObject) resource = Serializer.Json.Deserialize<TResource>(jsonObject);
            else resource = Serializer.Json.Deserialize<TResource>(Serializer.Json.Serialize(resource));
            if (resource == null) return null;
        }
        return new(e.Type, e.Group, e.Version, e.Plural, resource);
    }

}