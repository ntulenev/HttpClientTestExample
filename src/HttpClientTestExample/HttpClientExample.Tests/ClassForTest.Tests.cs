using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

using HttpClientTestExample;
using Moq;

namespace HttpClientExample.Tests
{
    public class ExampleTests
    {
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
                                                                   CancellationToken cancellationToken)
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

        [Fact(DisplayName = "Request should return correct content length.")]
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
            var handlerMoq = new Mock<HttpMessageHandler>();
            HttpClient client = new HttpClient(handlerMoq.Object);

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
            var handlerMoq = new Mock<HttpMessageHandler>();
            HttpClient client = new HttpClient(handlerMoq.Object);
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
            var handlerMoq = new Mock<HttpMessageHandler>();
            HttpClient client = new HttpClient(handlerMoq.Object);
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
            var handlerMoq = new Mock<HttpMessageHandler>();
            HttpClient client = new HttpClient(handlerMoq.Object);
            var testClass = new ClassForTest(client);
            string uri = "     ";

            // Act
            var exception = await Record.ExceptionAsync(
                async () => await testClass.GetContentLengthAsync(uri, CancellationToken.None));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }
    }
}
