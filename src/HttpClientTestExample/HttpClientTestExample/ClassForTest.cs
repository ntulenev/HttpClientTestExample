﻿namespace HttpClientTestExample;

/// <summary>
/// Class for example of test with mocking of http client.
/// </summary>
public class ClassForTest
{
    /// <summary>
    /// Creates <see cref="ClassForTest"/>
    /// </summary>
    /// <param name="httpClient">Http client for test.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ClassForTest(HttpClient httpClient)
    {
        _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Calculated web resource string length.
    /// </summary>
    /// <param name="uri">web resource.</param>
    /// <param name="ct">token.</param>
    /// <returns>length of content.</returns>
    public async Task<int> GetContentLengthAsync(string uri, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(uri))
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new ArgumentException("Uri can't contains only whitespaces.", nameof(uri));
        }

        var data = await _client.GetStringAsync(uri, ct);

        return data.Length;
    }

    private readonly HttpClient _client;
}
