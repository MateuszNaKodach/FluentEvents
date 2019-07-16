using System.Net;
using System.Net.Http;
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
        private readonly string _hubName = "hubName";
        private readonly PublicationMethod _publicationMethod = PublicationMethod.Group;
        private readonly string _hubMethodName = "hubMethodName";
        private readonly string[] _receiverIds = { "1", "2" };
        private readonly object _eventSender = new object();
        private readonly object _eventArgs = new object();
        private readonly string _endpoint = "endpoint";

        private AzureSignalRServiceOptions _options;
        private Mock<IAzureSignalRHttpClient> _azureSignalRHttpClientMock;
        private Mock<IUrlProvider> _urlProviderMock;
        private Mock<IHttpRequestFactory> _httpRequestFactoryMock;
        private EventSendingService _eventSendingService;

        [SetUp]
        public void SetUp()
        {
            _options = new AzureSignalRServiceOptions
            {
                ConnectionString = new ConnectionString(_endpoint, "key")
            };
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
        public async Task SendEventAsync_WithReceiverIds_ShouldSendHttpRequests()
        {
            foreach (var receiverId in _receiverIds)
            {
                var url = "url" + receiverId;

                SetUpGetUrl(receiverId, url);

                var httpRequestMessage = SetUpCreateHttpRequest(url);

                SetUpSendAsync(httpRequestMessage, HttpStatusCode.Accepted);
            }

            await _eventSendingService.SendEventAsync(
                _publicationMethod,
                _hubName,
                _hubMethodName,
                _receiverIds,
                _eventArgs
            );
        }

        [Test]
        public async Task SendEventAsync_NullWithReceiverIds_ShouldSendHttpRequest()
        {
            var url = "url";

            SetUpGetUrl(null, url);

            var httpRequestMessage = SetUpCreateHttpRequest(url);

            SetUpSendAsync(httpRequestMessage, HttpStatusCode.Accepted);

            await _eventSendingService.SendEventAsync(
                _publicationMethod,
                _hubName,
                _hubMethodName,
                null,
                _eventArgs
            );
        }


        [Test]
        public void SendEventAsync_WithHttpException_ShouldThrow()
        {
            var url = "url";

            SetUpGetUrl(null, url);

            var httpRequestMessage = SetUpCreateHttpRequest(url);

            SetUpSendAsync(httpRequestMessage, HttpStatusCode.ServiceUnavailable);

            Assert.That(async () =>
            {
                await _eventSendingService.SendEventAsync(
                    _publicationMethod,
                    _hubName,
                    _hubMethodName,
                    null,
                    _eventArgs
                );
            }, Throws.TypeOf<AzureSignalRPublishingFailedException>());
        }

        [Test]
        public void SendEventAsync_WithStatusCodeDifferentFromAccepted_ShouldThrow()
        {
            var url = "url";

            SetUpGetUrl(null, url);

            var httpRequestMessage = SetUpCreateHttpRequest(url);

            SetUpSendAsync(httpRequestMessage, HttpStatusCode.OK);

            Assert.That(async () =>
            {
                await _eventSendingService.SendEventAsync(
                    _publicationMethod,
                    _hubName,
                    _hubMethodName,
                    null,
                    _eventArgs
                );
            }, Throws.TypeOf<AzureSignalRPublishingFailedException>());
        }

        private void SetUpGetUrl(string receiverId, string url)
        {
            _urlProviderMock
                .Setup(x => x.GetUrl(_endpoint, _publicationMethod, _hubName, receiverId))
                .Returns(url)
                .Verifiable();
        }

        private HttpRequestMessage SetUpCreateHttpRequest(string url)
        {
            var httpRequestMessage = new HttpRequestMessage();

            _httpRequestFactoryMock
                .Setup(x => x.CreateHttpRequest(It.IsAny<ConnectionString>(), _hubMethodName, _eventArgs, url))
                .Returns(httpRequestMessage)
                .Verifiable();
            return httpRequestMessage;
        }

        private void SetUpSendAsync(HttpRequestMessage httpRequestMessage, HttpStatusCode httpStatusCode)
        {
            var httpResponseMessage = new HttpResponseMessage { StatusCode = httpStatusCode };

            _azureSignalRHttpClientMock
                .Setup(x => x.SendAsync(httpRequestMessage))
                .Returns(Task.FromResult(httpResponseMessage))
                .Verifiable();
        }
    }
}