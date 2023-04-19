namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to load and manage <see cref="IPlugin"/>s
/// </summary>
public interface IPluginManager
{

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing all loaded <see cref="IPlugin"/>s
    /// </summary>
    IEnumerable<IPlugin> Plugins { get; }

}
