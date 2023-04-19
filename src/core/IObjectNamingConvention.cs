namespace Hylo;

/// <summary>
/// Defines the fundamentals of a service used to validate object names
/// </summary>
public interface IObjectNamingConvention
{

    /// <summary>
    /// Determines whether or not the specified resource group is valid
    /// </summary>
    /// <param name="group">The resource group to validate</param>
    /// <returns>A boolean indicating whether or not the specified value is valid</returns>
    bool IsValidResourceGroup(string group);

    /// <summary>
    /// Determines whether or not the specified resource name is valid
    /// </summary>
    /// <param name="name">The resource name to validate</param>
    /// <returns>A boolean indicating whether or not the specified value is valid</returns>
    bool IsValidResourceName(string name);

    /// <summary>
    /// Determines whether or not the specified resource plural name is valid
    /// </summary>
    /// <param name="plural">The resource plural name to validate</param>
    /// <returns>A boolean indicating whether or not the specified value is valid</returns>
    bool IsValidResourcePluralName(string plural);

    /// <summary>
    /// Determines whether or not the specified resource kind is valid
    /// </summary>
    /// <param name="kind">The resource kind to validate</param>
    /// <returns>A boolean indicating whether or not the specified value is valid</returns>
    bool IsValidResourceKind(string kind);

    /// <summary>
    /// Determines whether or not the specified version is valid
    /// </summary>
    /// <param name="version">The version to validate</param>
    /// <returns>A boolean indicating whether or not the specified value is valid</returns>
    bool IsValidVersion(string version);

    /// <summary>
    /// Determines whether or not the specified annotation name is valid
    /// </summary>
    /// <param name="annotation">The annotation name to validate</param>
    /// <returns>A boolean indicating whether or not the specified value is valid</returns>
    bool IsValidAnnotationName(string annotation);

    /// <summary>
    /// Determines whether or not the specified label name is valid
    /// </summary>
    /// <param name="label">The label name to validate</param>
    /// <returns>A boolean indicating whether or not the specified value is valid</returns>
    bool IsValidLabelName(string label);

}
