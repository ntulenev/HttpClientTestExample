using FluentAssertions;

using Xunit;

using HttpClientTestExample;

using Moq;
using Moq.Protected;

namespace HttpClientExample.Tests;

public class ExampleTests
{
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

    [Fact(DisplayName = "ClassForTest can't be constructed with null HttpClient.")]
    [Trait("Category", "Unit")]
    public void CantBeConstructedNullClient()
    {
        // Arrange
        HttpClient client = null!;

        // Act
        var exception = Record.Exception(
            () => new ClassForTest(client));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "ClassForTest can't be constructed with null HttpClient.")]
    [Trait("Category", "Unit")]
    public void CanBeConstructedValidClient()
    {
        // Arrange
        var handlerMoq = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var client = new HttpClient(handlerMoq.Object);

        // Act
        var exception = Record.Exception(
            () => new ClassForTest(client));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "ClassForTest can't send message for null uri.")]
    [Trait("Category", "Unit")]
    public async Task CantSendMessageOnNullUriAsync()
    {
        // Arrange
        var handlerMoq = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var client = new HttpClient(handlerMoq.Object);
        var testClass = new ClassForTest(client);
        string uri = null!;

        // Act
        var exception = await Record.ExceptionAsync(
            async () => await testClass.GetContentLengthAsync(uri, CancellationToken.None));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "ClassForTest can't send message for empty uri.")]
    [Trait("Category", "Unit")]
    public async Task CantSendMessageOnEmptyUriAsync()
    {
        // Arrange
        var handlerMoq = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var client = new HttpClient(handlerMoq.Object);
        var testClass = new ClassForTest(client);
        string uri = string.Empty;

        // Act
        var exception = await Record.ExceptionAsync(
            async () => await testClass.GetContentLengthAsync(uri, CancellationToken.None));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "ClassForTest can't send message for uri of whitespaces.")]
    [Trait("Category", "Unit")]
    public async Task CantSendMessageOnAllWhitespacesUriAsync()
    {
        // Arrange
        var handlerMoq = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var client = new HttpClient(handlerMoq.Object);
        var testClass = new ClassForTest(client);
        string uri = "     ";

        // Act
        var exception = await Record.ExceptionAsync(
            async () => await testClass.GetContentLengthAsync(uri, CancellationToken.None));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
    }
}
