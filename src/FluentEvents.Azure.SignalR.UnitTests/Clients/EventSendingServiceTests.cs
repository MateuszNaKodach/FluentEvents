using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentEvents.Azure.SignalR.Client;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests.Clients
{
    [TestFixture]
    public class EventSendingServiceTests
    {
        private AzureSignalRServiceConfig _options;
        private Mock<IAzureSignalRHttpClient> _azureSignalRHttpClientMock;
        private Mock<IUrlProvider> _urlProviderMock;
        private Mock<IHttpRequestFactory> _httpRequestFactoryMock;
        private EventSendingService _eventSendingService;

        [SetUp]
        public void SetUp()
        {
            _options = new AzureSignalRServiceConfig();
            _azureSignalRHttpClientMock = new Mock<IAzureSignalRHttpClient>(MockBehavior.Strict);
            _urlProviderMock = new Mock<IUrlProvider>(MockBehavior.Strict);
            _httpRequestFactoryMock = new Mock<IHttpRequestFactory>(MockBehavior.Strict);

            _eventSendingService = new EventSendingService(
                Options.Create(_options),
                _azureSignalRHttpClientMock.Object,
                _urlProviderMock.Object,
                _httpRequestFactoryMock.Object
            );
        }

        [Test]
        public async Task SendEvent_ShouldSendHttpRequest()
        {
            var publicationMethod = PublicationMethod.Group;
            var hubName = "hubName";
            var hubMethodName = "hubMethodName";
            var receiverIds = new [] { "1", "2" };
            var eventSender = new object();
            var eventArgs = new object();

            var endpoint = "endpoint";
            var url = "url";

            _urlProviderMock
                .Setup(x => x.GetUrl(endpoint, publicationMethod, hubName, "1"))
                .Returns(url)
                .Verifiable();

            var httpRequestMessage = new HttpRequestMessage();

            _httpRequestFactoryMock
                .Setup(x => x.CreateHttpRequest(_options.ConnectionString, hubMethodName, eventSender, eventArgs, url))
                .Returns(httpRequestMessage)
                .Verifiable();

            var httpResponseMessage = new HttpResponseMessage {StatusCode = HttpStatusCode.OK};

            _azureSignalRHttpClientMock
                .Setup(x => x.SendAsync(httpRequestMessage))
                .Returns(Task.FromResult(httpResponseMessage))
                .Verifiable();

            await _eventSendingService.SendEventAsync(
                publicationMethod,
                hubName,
                hubMethodName,
                receiverIds,
                eventSender,
                eventArgs
            );
        }
    }
}
