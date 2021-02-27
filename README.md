# HttpClientTestExample

### HttpClient process requests via implementation of abstract class HttpMessageHandler.

#### It can be easily changed on stub for tests.

Simplified naive implementation

```C#
/// <summary>
/// HttpMessageHandler mock
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    public MockHttpMessageHandler(string content, string method, string uri)
    {
        _content = content;
        _method = method;
        _uri = uri;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken cancellationToken
    {
        if (request.RequestUri!.ToString() == _uri && request.Method.ToString() == _method)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                Content = new StringContent(_content)
            });
        }
        else
        {
            throw new InvalidOperationException("Invalid params");
        }
    }

    private readonly string _content;
    private readonly string _uri;
    private readonly string _method;
}
```

Using in test
```C#
[Fact(DisplayName = "Web request should return correct content length.")]
[Trait("Category", "Unit")]
public async Task CanReadContentWithValidParamsAsync()
{
    // Arrange
    var testString = "test123";
    var uri = "https://www.test.com/";
    var handler = new MockHttpMessageHandler(testString, "GET", uri);
    var client = new HttpClient(handler);
    var testClass = new ClassForTest(client);

    var result = 0;
    // Act
    var exception = await Record.ExceptionAsync(
        async () => result = await testClass.GetContentLengthAsync(uri, CancellationToken.None));

    // Assert
    exception.Should().BeNull();
    result.Should().Be(testString.Length);

}
```
