namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to provide information about users
/// </summary>
public interface IUserInfoProvider
{

    /// <summary>
    /// Gets information about the current user, if any
    /// </summary>
    /// <returns>An object that describes the current user</returns>
    UserInfo? GetCurrentUser();

}
