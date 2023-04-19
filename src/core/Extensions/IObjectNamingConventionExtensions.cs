﻿namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="IObjectNamingConvention"/>s
/// </summary>
public static class IObjectNamingConventionExtensions
{

    /// <summary>
    /// Ensures that the specified value is valid
    /// </summary>
    /// <param name="namingConvention">The extended <see cref="IObjectNamingConvention"/></param>
    /// <param name="group">The resource group to validate</param>
    public static void EnsureIsValidResourceGroup(this IObjectNamingConvention namingConvention, string group)
    {
        if (!namingConvention.IsValidResourceGroup(group)) throw new ArgumentException("Invalid format", nameof(group));
    }

    /// <summary>
    /// Ensures that the specified value is valid
    /// </summary>
    /// <param name="namingConvention">The extended <see cref="IObjectNamingConvention"/></param>
    /// <param name="name">The resource name to validate</param>
    public static void EnsureIsValidResourceName(this IObjectNamingConvention namingConvention, string name)
    {
        if (!namingConvention.IsValidResourceName(name)) throw new ArgumentException("Invalid format", nameof(name));
    }

    /// <summary>
    /// Ensures that the specified value is valid
    /// </summary>
    /// <param name="namingConvention">The extended <see cref="IObjectNamingConvention"/></param>
    /// <param name="plural">The resource name to validate</param>
    public static void EnsureIsValidResourcePluralName(this IObjectNamingConvention namingConvention, string plural)
    {
        if (!namingConvention.IsValidResourcePluralName(plural)) throw new ArgumentException("Invalid format", nameof(plural));
    }

    /// <summary>
    /// Ensures that the specified value is valid
    /// </summary>
    /// <param name="namingConvention">The extended <see cref="IObjectNamingConvention"/></param>
    /// <param name="kind">The resource name to validate</param>
    public static void EnsureIsValidResourceKind(this IObjectNamingConvention namingConvention, string kind)
    {
        if (!namingConvention.IsValidResourceKind(kind)) throw new ArgumentException("Invalid format", nameof(kind));
    }

    /// <summary>
    /// Ensures that the specified value is valid
    /// </summary>
    /// <param name="namingConvention">The extended <see cref="IObjectNamingConvention"/></param>
    /// <param name="version">The version to validate</param>
    public static void EnsureIsValidVersion(this IObjectNamingConvention namingConvention, string version)
    {
        if (!namingConvention.IsValidVersion(version)) throw new ArgumentException("Invalid format", nameof(version));
    }

    /// <summary>
    /// Ensures that the specified value is valid
    /// </summary>
    /// <param name="namingConvention">The extended <see cref="IObjectNamingConvention"/></param>
    /// <param name="annotationName">The annotation name to validate</param>
    public static void EnsureIsValidAnnotationName(this IObjectNamingConvention namingConvention, string annotationName)
    {
        if (!namingConvention.IsValidAnnotationName(annotationName)) throw new ArgumentException("Invalid format", nameof(annotationName));
    }

    /// <summary>
    /// Ensures that the specified value is valid
    /// </summary>
    /// <param name="namingConvention">The extended <see cref="IObjectNamingConvention"/></param>
    /// <param name="labelName">The label name to validate</param>
    public static void EnsureIsValidLabelName(this IObjectNamingConvention namingConvention, string labelName)
    {
        if (!namingConvention.IsValidLabelName(labelName)) throw new ArgumentException("Invalid format", nameof(labelName));
    }

}