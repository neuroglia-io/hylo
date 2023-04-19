﻿namespace Hylo;

/// <summary>
/// Represents the default implementation of the <see cref="IObjectNamingConvention"/> interface
/// </summary>
public class ObjectNamingConvention
    : IObjectNamingConvention
{

    readonly int _groupMaxLength = 256;
    readonly int _nameMaxLength = 256;
    readonly int _annotationMaxLength = 63;
    readonly int _labelMaxLength = 63;
    readonly int _maxVersionLength = 22;

    /// <summary>
    /// Gets/sets the current <see cref="IObjectNamingConvention"/> for Hylo object names
    /// </summary>
    public static IObjectNamingConvention Current { get; set; } = new ObjectNamingConvention();

    /// <inheritdoc/>
    public virtual bool IsValidResourceGroup(string group)
    {
        if (string.IsNullOrWhiteSpace(group)) return true;
        return group.Length <= this._groupMaxLength
            && group.IsLowercased()
            && group.IsAlphanumeric('.', '-')
            && char.IsLetterOrDigit(group.First())
            && char.IsLetterOrDigit(group.Last());
    }

    /// <inheritdoc/>
    public virtual bool IsValidResourceName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        return name.Length <= this._nameMaxLength
            && name.IsLowercased()
            && name.IsAlphanumeric('-')
            && char.IsLetterOrDigit(name.First())
            && char.IsLetterOrDigit(name.Last());
    }

    /// <inheritdoc/>
    public virtual bool IsValidResourcePluralName(string plural)
    {
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        return plural.Length <= this._nameMaxLength
            && plural.IsLowercased()
            && plural.IsAlphanumeric('-')
            && char.IsLetterOrDigit(plural.First())
            && char.IsLetterOrDigit(plural.Last());
    }

    /// <inheritdoc/>
    public virtual bool IsValidResourceKind(string kind)
    {
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        return kind.Length <= this._nameMaxLength
            && kind.IsAlphabetic()
            && char.IsUpper(kind.First());
    }
    /// <inheritdoc/>
    public virtual bool IsValidAnnotationName(string annotation)
    {
        if (string.IsNullOrWhiteSpace(annotation)) throw new ArgumentNullException(nameof(annotation));
        return annotation.Length <= this._annotationMaxLength
            && annotation.IsLowercased()
            && annotation.IsAlphanumeric('-')
            && char.IsAsciiLetterOrDigit(annotation.First())
            && char.IsLetterOrDigit(annotation.Last());
    }

    /// <inheritdoc/>
    public virtual bool IsValidLabelName(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentNullException(nameof(label));
        return label.Length <= this._labelMaxLength
            && label.IsLowercased()
            && label.IsAlphanumeric('-')
            && char.IsAsciiLetterOrDigit(label.First())
            && char.IsLetterOrDigit(label.Last());
    }

    /// <inheritdoc/>
    public virtual bool IsValidVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        return version.Length <= this._maxVersionLength
            && version.IsAlphanumeric()
            && version.StartsWith('v');
    }

}