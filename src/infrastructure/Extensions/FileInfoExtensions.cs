namespace Hylo.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="FileInfo"/>s
/// </summary>
public static class FileInfoExtensions
{

    /// <summary>
    /// Determines whether or not the <see cref="FileInfo"/> is locked
    /// </summary>
    /// <param name="file">The <see cref="FileInfo"/> to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="FileInfo"/> is locked</returns>
    /// <remarks>Code taken from <see href="https://stackoverflow.com/a/10982239/3637555"/></remarks>
    public static bool IsLocked(this FileInfo file)
    {
        var stream = null as FileStream;
        try
        {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            return true;
        }
        finally
        {
            stream?.Close();
        }
        return false;
    }

}
