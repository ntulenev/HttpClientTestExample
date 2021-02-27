# HttpClient Test example

### HttpClient handles requests via implementation of abstract class HttpMessageHandler.

#### It can be easily changed with stub for tests.

Simplified naive implementation

```C#
var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
     "SendAsync",
     ItExpr.Is<HttpRequestMessage>(a => a.Method.ToString() == "GET" && a.RequestUri!.ToString() == uri),
     ItExpr.IsAny<CancellationToken>()
 )
.ReturnsAsync(new HttpResponseMessage
{
    Content = new StringContent(testString)
});
```

Test example 

```C#
[Fact(DisplayName = "Request should return correct content length.")]
[Trait("Category", "Unit")]
public async Task CanReadContentWithValidParamsAsync()
{
    // Arrange
    var testString = "test123";
    var uri = "https://www.test.com/";

    var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(a => a.Method.ToString() == "GET" && a.RequestUri!.ToString() == uri),
            ItExpr.IsAny<CancellationToken>()
        )
       .ReturnsAsync(new HttpResponseMessage
       {
           Content = new StringContent(testString)
       });

    var client = new HttpClient(handlerMock.Object);
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
