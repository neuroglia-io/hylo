namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of a <see cref="BackgroundService"/> used to load and manage <see cref="IPlugin"/>s
/// </summary>
public class PluginManager
    : BackgroundService, IPluginManager
{

    /// <summary>
    /// Gets the name of an <see cref="IPlugin"/> metadata file
    /// </summary>
    public const string PluginMetadataFileName = "plugin.json";

    /// <summary>
    /// Initializes a new <see cref="PluginManager"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    public PluginManager(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets an <see cref="SynchronizedCollection{T}"/> containing the <see cref="IPluginController"/>s of all discovered <see cref="IPlugin"/>s
    /// </summary>
    public SynchronizedCollection<IPluginController> PluginControllers { get; } = new();

    /// <inheritdoc/>
    public IEnumerable<IPlugin> Plugins => this.PluginControllers.Where(c => c.Plugin != null).Select(c => c.Plugin!).ToList();

    /// <summary>
    /// Gets the <see cref="PluginManager"/>'s <see cref="CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    /// <summary>
    /// Gets the service used to watch the <see cref="IPlugin"/> files
    /// </summary>
    protected FileSystemWatcher FileSystemWatcher { get; private set; } = null!;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        var pluginDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "plugins"));
        if (!pluginDirectory.Exists) pluginDirectory.Create();
        foreach (var packageFile in pluginDirectory.GetFiles("*.tar.gz"))
        {
            await TarGzPackage.ExtractToDirectoryAsync(packageFile.FullName, packageFile.Directory!.FullName, this.CancellationTokenSource.Token).ConfigureAwait(false);
            packageFile.Delete();
        }
        var plugins = await this.FindPluginsAsync(pluginDirectory.FullName);
        foreach (var plugin in plugins)
        {
            this.Logger.LogInformation("Loading plugin '{plugin}'...", plugin.ToString());
            try
            {
                await plugin.LoadAsync(this.CancellationTokenSource.Token);
                this.Logger.LogInformation("The plugin '{plugin}' has been successfully loaded", plugin.ToString());
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning("An error occured while loading plugin '{plugin}': {ex}", plugin.ToString(), ex.ToString());
                continue;
            }
        }
        this.FileSystemWatcher = new(pluginDirectory.FullName, $"*.*")
        {
            IncludeSubdirectories = true
        };
        this.FileSystemWatcher.Created += this.OnPluginFileCreatedAsync;
        this.FileSystemWatcher.Deleted += this.OnPluginFileDeletedAsync;
        this.FileSystemWatcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Finds all <see cref="IPlugin"/>s in the specified directory
    /// </summary>
    /// <param name="directoryPath">The path of the directory to scan for <see cref="IPlugin"/>s</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing <see cref="IPluginController"/>s used to control the <see cref="IPlugin"/>s that have been found</returns>
    public virtual async Task<IEnumerable<IPluginController>> FindPluginsAsync(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath)) throw new ArgumentNullException(nameof(directoryPath));
        this.Logger.LogInformation("Scanning directory'{directory}' for plugins...", directoryPath);
        var directory = new DirectoryInfo(directoryPath);
        if (!directory.Exists) throw new DirectoryNotFoundException($"Failed to find the specified directory '{directoryPath}'");
        var pluginFiles = directory.GetFiles(PluginMetadataFileName, SearchOption.AllDirectories);
        this.Logger.LogInformation("Found {results} matching plugin files in directory '{directory}'", pluginFiles.Length, directoryPath);
        var plugins = new List<IPluginController>(pluginFiles.Length);
        foreach (var pluginFile in pluginFiles)
        {
            plugins.Add(await this.FindPluginAsync(pluginFile.FullName));
        }
        this.Logger.LogInformation("{pluginCount} plugins have been found in '{directory}' directory", plugins.Count, directoryPath);
        return plugins;
    }

    /// <summary>
    /// Finds the <see cref="IPlugin"/> at the specified file path
    /// </summary>
    /// <param name="metadataFilePath">The file path of the <see cref="IPlugin"/> to find</param>
    /// <returns>A new <see cref="IPluginController"/></returns>
    public virtual Task<IPluginController> FindPluginAsync(string metadataFilePath)
    {
        if (string.IsNullOrWhiteSpace(metadataFilePath)) throw new ArgumentNullException(nameof(metadataFilePath));
        var pluginController = this.PluginControllers.FirstOrDefault(p => p.MetadataFilePath == metadataFilePath);
        if (pluginController != null) return Task.FromResult(pluginController);
        var file = new FileInfo(metadataFilePath);
        if (!file.Exists) throw new DirectoryNotFoundException($"Failed to find the specified file '{metadataFilePath}'");
        if (file.Name != PluginMetadataFileName) throw new Exception($"The specified file '{metadataFilePath}' is not a valid plugin metadata file");
        var pluginMetadata = Serializer.Json.Deserialize<PluginMetadata>(File.ReadAllText(file.FullName))!;
        pluginController = ActivatorUtilities.CreateInstance<PluginController>(this.ServiceProvider, pluginMetadata, metadataFilePath);
        pluginController.Disposed += (sender, e) => this.PluginControllers.Remove((IPluginController)sender!);
        this.PluginControllers.Add(pluginController);
        return Task.FromResult(pluginController);
    }

    /// <summary>
    /// Handles the creation of a new <see cref="IPlugin"/> file
    /// </summary>
    /// <param name="sender">The service used to watch the <see cref="IPlugin"/> files</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> to handle</param>
    protected virtual async void OnPluginFileCreatedAsync(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.EndsWith(".tar.gz"))
        {
            var packageFile = new FileInfo(e.FullPath);
            do
            {
                await Task.Delay(10).ConfigureAwait(false);
            }
            while (packageFile.IsLocked());
            using var packageFileStream = packageFile.OpenRead();
            await TarGzPackage.ExtractToDirectoryAsync(packageFile.FullName, packageFile.Directory!.FullName).ConfigureAwait(false);
            await packageFileStream.DisposeAsync().ConfigureAwait(false);
            await this.FindPluginsAsync(packageFile.Directory!.FullName).ConfigureAwait(false);
            packageFile.Delete();
        }
        else if (e.FullPath == "plugin.json")
        {
            await this.FindPluginAsync(e.FullPath).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Handles the deletion of a new <see cref="IPlugin"/> file
    /// </summary>
    /// <param name="sender">The service used to watch the <see cref="IPlugin"/> files</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> to handle</param>
    protected virtual async void OnPluginFileDeletedAsync(object sender, FileSystemEventArgs e)
    {
        var pluginController = this.PluginControllers.FirstOrDefault(p => p.MetadataFilePath == e.FullPath);
        if (pluginController != null) await pluginController.DisposeAsync().ConfigureAwait(false);
    }

}
