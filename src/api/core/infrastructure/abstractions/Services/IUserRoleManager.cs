namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage user <see cref="V1Role"/>s
/// </summary>
public interface IUserRoleManager
{

    /// <summary>
    /// Retrieves all the <see cref="V1Role"/>s assigned to the specified user
    /// </summary>
    /// <param name="subject">The subject to list the <see cref="V1Role"/>s of</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the names of the <see cref="V1Role"/> assigned to the specified user</returns>
    IAsyncEnumerable<string> GetRolesAsync(string subject, CancellationToken cancellationToken = default);

}
