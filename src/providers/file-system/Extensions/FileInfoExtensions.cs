namespace Hylo.Providers.FileSystem;

/// <summary>
/// Defines extensions for <see cref="FileInfo"/>s
/// </summary>
public static class FileInfoExtensions
{

    /// <summary>
    /// Awaits the specified file to become awailable the open it for writing
    /// </summary>
    /// <param name="file">The file to open</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The specified file's stream</returns>
    public static async Task<FileStream> OpenWriteAsync(this FileInfo file, CancellationToken cancellationToken = default)
    {
        do
        {
            try
            {
                return file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException ex) when (ex is not FileNotFoundException && ex is not DirectoryNotFoundException) 
            {
                await Task.Delay(5, cancellationToken).ConfigureAwait(false);
            }
        }
        while (!cancellationToken.IsCancellationRequested);
        return null!;
    }

    /// <summary>
    /// Awaits the specified file to become awailable the open it for reading
    /// </summary>
    /// <param name="file">The file to open</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The specified file's stream</returns>
    public static async Task<FileStream> OpenReadAsync(this FileInfo file, CancellationToken cancellationToken = default)
    {
        do
        {
            try
            {
                return file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException ex) when (ex is not FileNotFoundException && ex is not DirectoryNotFoundException)
            {
                await Task.Delay(5, cancellationToken).ConfigureAwait(false);
            }
        }
        while (!cancellationToken.IsCancellationRequested);
        return null!;

    }

}
