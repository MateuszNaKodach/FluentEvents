using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentEvents.Azure.SignalR.Client;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests.Clients
{
    [TestFixture]
    public class HttpRequestFactoryTests
    {
        private readonly string _hubMethodName = nameof(_hubMethodName);
        private readonly string _url = "http://endpoint/";
        private readonly ConnectionString _connectionString = "Endpoint=123;AccessKey=123;";

        private Mock<IAccessTokensService> _accessTokensServiceMock;
        private object _event;

        private HttpRequestFactory _httpRequestFactory;

        [SetUp]
        public void SetUp()
        {
            _accessTokensServiceMock = new Mock<IAccessTokensService>(MockBehavior.Strict);
            _event = new object();

            _httpRequestFactory = new HttpRequestFactory(_accessTokensServiceMock.Object);
        }

        [Test]
        public async Task CreateHttpRequest_ShouldCreateHttpRequest()
        {
            var accessToken = "accessToken";
            _accessTokensServiceMock
                .Setup(x => x.GenerateAccessToken(_connectionString, _url, null, null))
                .Returns(accessToken)
                .Verifiable();

            var httpRequest = _httpRequestFactory.CreateHttpRequest(
                _connectionString,
                _hubMethodName,
                _event,
                _url
            );

            Assert.That(httpRequest, Is.Not.Null);
            Assert.That(httpRequest, Has.Property(nameof(HttpRequestMessage.Method)).EqualTo(HttpMethod.Post));
            Assert.That(
                httpRequest,
                Has.Property(nameof(HttpRequestMessage.RequestUri)).Property(nameof(Uri.AbsoluteUri)).EqualTo(_url)
            );

            AssertThatHasHeader(httpRequest, "Authorization", "Bearer " + accessToken);
            AssertThatHasHeader(httpRequest, "Accept", "application/json");

            var content = await httpRequest.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(content);

            Assert.That(jObject["Target"].Value<string>(), Is.EqualTo(_hubMethodName));
            Assert.That(jObject["Arguments"], Is.Not.Null);
        }

        private static void AssertThatHasHeader(HttpRequestMessage httpRequest, string name, string value)
        {
            Assert.That(
                httpRequest,
                Has.Property(nameof(HttpRequestMessage.Headers))
                    .With.One.Items
                    .With.Property(nameof(KeyValuePair<object, object>.Key))
                    .EqualTo(name)
                    .And.Property(nameof(KeyValuePair<object, object>.Value))
                    .EquivalentTo(new[] { value })
            );
        }
    }
}
