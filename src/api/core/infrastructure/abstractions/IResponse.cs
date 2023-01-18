namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Defines the fundamentals of a response to an <see cref="IRequest"/>
/// </summary>
public interface IResponse
    : IExtensible
{

    /// <summary>
    /// Gets the <see cref="IResponse"/>'s status code
    /// </summary>
    int Status { get; set; }

    /// <summary>
    /// Gets an URI referencing a document that provides human-readable documentation for the response type
    /// </summary>
    Uri? Type { get; set; }

    /// <summary>
    /// A short, human-readable summary of the response type. It SHOULD NOT change from occurrence to occurrence
    /// of the response, except for purposes of localization(e.g., using proactive content negotiation;
    /// see[RFC7231], Section 3.4).
    /// </summary>
    string? Title { get; set; }

    /// <summary>
    /// Gets a human-readable explanation specific to this occurrence of the response.
    /// </summary>
    string? Detail { get; set; }

    /// <summary>
    /// Gets an URI reference that identifies the specific occurrence of the response. It may or may not yield further information if dereferenced.
    /// </summary>
    Uri? Instance { get; set; }

    /// <summary>
    /// Gets the <see cref="IResponse"/>'s content, if any
    /// </summary>
    object? Content { get; set; }

    /// <summary>
    /// Gets an <see cref="IDictionary{TKey, TValue}"/> containing they code/message mappings of the errors that have occured during the <see cref="IRequest"/>'s execution
    /// </summary>
    IDictionary<string, string[]>? Errors { get; set; }

}

/// <summary>
/// Defines the fundamentals of a response to an <see cref="IRequest"/>
/// </summary>
/// <typeparam name="TContent">The type of content returned by the <see cref="IRequest"/></typeparam>
public interface IResponse<TContent>
    : IResponse
{

    /// <summary>
    /// Gets the <see cref="IResponse"/>'s content, if any
    /// </summary>
    new TContent? Content { get; set; }

}
