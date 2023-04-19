using Json.Pointer;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Hylo.Serialization.Yaml;

/// <summary>
/// Represents the <see cref="IYamlTypeConverter"/> used to convert <see cref="JsonPointer"/>s
/// </summary>
public class JsonPointerTypeConverter
    : IYamlTypeConverter
{

    /// <inheritdoc/>
    public virtual bool Accepts(Type type) => typeof(JsonPointer).IsAssignableFrom(type);

    /// <inheritdoc/>
    public virtual object? ReadYaml(IParser parser, Type type) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        var pointer = value as JsonPointer;
        if(pointer == null) return;
        emitter.Emit(new Scalar(pointer.ToString()));
    }

}