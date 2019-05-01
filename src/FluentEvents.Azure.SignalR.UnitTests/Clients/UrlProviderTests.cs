using FluentEvents.Azure.SignalR.Client;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests.Clients
{
    [TestFixture]
    public class UrlProviderTests
    {
        private const string Endpoint = "http://endpoint";
        private const string HubName = nameof(HubName);
        private const string ReceiverId = nameof(ReceiverId);

        private UrlProvider _urlProvider;

        [SetUp]
        public void SetUp()
        {
            _urlProvider = new UrlProvider();
        }

        [Test]
        public void GetUrl_ShouldTrimEndpointSlash()
        {
            var url = _urlProvider.GetUrl(Endpoint + "/", PublicationMethod.Broadcast, HubName, null);

            Assert.That(url, Is.EqualTo(Endpoint + $"/api/v1/hubs/{HubName.ToLower()}"));
        }

        [Test]
        public void GetUrl_WithPublicationTypeBroadcast_ShouldReturnCorrectUrl()
        {
            var url = _urlProvider.GetUrl(Endpoint, PublicationMethod.Broadcast, HubName, null);
            
            Assert.That(url, Is.EqualTo(Endpoint + $"/api/v1/hubs/{HubName.ToLower()}"));
        }

        [Test]
        public void GetUrl_WithPublicationTypeUser_ShouldReturnCorrectUrl()
        {
            var url = _urlProvider.GetUrl(Endpoint, PublicationMethod.User, HubName, ReceiverId);

            Assert.That(url, Is.EqualTo(Endpoint + $"/api/v1/hubs/{HubName.ToLower()}/users/{ReceiverId}"));
        }

        [Test]
        public void GetUrl_WithPublicationTypeGroup_ShouldReturnCorrectUrl()
        {
            var url = _urlProvider.GetUrl(Endpoint, PublicationMethod.Group, HubName, ReceiverId);

            Assert.That(url, Is.EqualTo(Endpoint + $"/api/v1/hubs/{HubName.ToLower()}/groups/{ReceiverId}"));
        }
    }
}
