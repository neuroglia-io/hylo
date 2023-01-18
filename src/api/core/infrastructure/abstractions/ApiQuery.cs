namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Represents the base class for all API <see cref="IQuery"/> implementations
/// </summary>
/// <typeparam name="TContent">The type of the expected <see cref="IQuery"/> <see cref="IResponse"/>'s content</typeparam>
public abstract class ApiQuery<TContent>
    : IQuery<IResponse<TContent>, TContent>
{

    IDictionary<string, object>? IQuery.Metadata { get; }

}
