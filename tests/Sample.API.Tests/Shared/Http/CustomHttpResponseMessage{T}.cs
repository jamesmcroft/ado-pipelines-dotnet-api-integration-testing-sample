using System.Net.Http;

namespace Sample.API.Tests.Shared.Http;

/// <summary>
/// Defines a wrapper for a <see cref="HttpResponseMessage"/> that contains the deserialized content.
/// </summary>
/// <typeparam name="T">The type of the deserialized content.</typeparam>
public class CustomHttpResponseMessage<T>(HttpResponseMessage response, T content)
{
    /// <summary>
    /// Gets the <see cref="HttpResponseMessage"/>.
    /// </summary>
    public HttpResponseMessage Response { get; } = response;

    /// <summary>
    /// Gets the deserialized content.
    /// </summary>
    public T Content { get; } = content;
}
