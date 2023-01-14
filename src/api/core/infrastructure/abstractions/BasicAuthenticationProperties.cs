namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Holds information about a <see cref="BasicAuthenticationDefaults.AuthenticationScheme"/> scheme authentication attempt
/// </summary>
public class BasicAuthenticationProperties
{

    /// <summary>
    /// Initializes a new <see cref="BasicAuthenticationProperties"/>
    /// </summary>
    /// <param name="username">The username to use</param>
    /// <param name="password">The password to use</param>
    public BasicAuthenticationProperties(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));
        this.Username = username;
        this.Password = new(password);
    }

    /// <summary>
    /// Gets the username to use
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the password to use
    /// </summary>
    public string Password { get; }

}
