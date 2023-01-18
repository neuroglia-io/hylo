namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Represents the base class for all API <see cref="ICommand"/>s
/// </summary>
public abstract class ApiCommand
    : ICommand<IResponse>
{

    IDictionary<string, object>? ICommand.Metadata { get; }

}

/// <summary>
/// Represents the base class for all API <see cref="ICommand"/>s
/// </summary>
/// <typeparam name="TContent">The type of the expected <see cref="ICommand"/> <see cref="IResponse"/>'s content</typeparam>
public abstract class V1ApiCommand<TContent>
    : ICommand<IResponse<TContent>, TContent>
{

    IDictionary<string, object>? ICommand.Metadata { get; }

}
