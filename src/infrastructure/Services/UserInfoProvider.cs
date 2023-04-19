namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IUserInfoProvider"/> interface
/// </summary>
public class UserInfoProvider
    : IUserInfoProvider
{

    /// <summary>
    /// Initializes a new <see cref="UserInfoProvider"/>
    /// </summary>
    /// <param name="userAccessor">The service used to access the current user</param>
    public UserInfoProvider(IUserAccessor userAccessor)
    {
        this.UserAccessor = userAccessor;
    }

    /// <summary>
    /// Gets the service used to access the current user
    /// </summary>
    protected IUserAccessor UserAccessor { get; }

    /// <summary>
    /// Gets information about the current user
    /// </summary>
    /// <returns>A new object used to provide information about the current user</returns>
    public virtual UserInfo? GetCurrentUser()
    {
        if (this.UserAccessor.User == null || this.UserAccessor.User.Identity?.IsAuthenticated != true) return null;
        return new(this.UserAccessor.User.Identity.Name!, this.UserAccessor.User.Identity.AuthenticationType!, this.UserAccessor.User.Claims.ToDictionary(c => c.Type, c => c.Value));
    }

}