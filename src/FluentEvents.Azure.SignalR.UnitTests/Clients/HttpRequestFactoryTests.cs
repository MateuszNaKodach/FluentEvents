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
        private readonly string m_HubMethodName = nameof(m_HubMethodName);
        private string m_Url = "http://endpoint/";
        private readonly ConnectionString m_ConnectionString = "Endpoint=123;AccessKey=123;";

        private Mock<IAccessTokensService> m_AccessTokensServiceMock;
        private object m_EventSender;
        private object m_EventArgs;

        private HttpRequestFactory m_HttpRequestFactory;

        [SetUp]
        public void SetUp()
        {
            m_AccessTokensServiceMock = new Mock<IAccessTokensService>(MockBehavior.Strict);
            m_EventSender = new object();
            m_EventArgs = new object();

            m_HttpRequestFactory = new HttpRequestFactory(m_AccessTokensServiceMock.Object);
        }

        [Test]
        public async Task CreateHttpRequest_ShouldCreateHttpRequest()
        {
            var accessToken = "accessToken";
            m_AccessTokensServiceMock
                .Setup(x => x.GenerateAccessToken(m_ConnectionString, m_Url, null, null))
                .Returns(accessToken)
                .Verifiable();

            var httpRequest = m_HttpRequestFactory.CreateHttpRequest(
                m_ConnectionString,
                m_HubMethodName,
                m_EventSender,
                m_EventArgs,
                m_Url
            );

            Assert.That(httpRequest, Is.Not.Null);
            Assert.That(httpRequest, Has.Property(nameof(HttpRequestMessage.Method)).EqualTo(HttpMethod.Post));
            Assert.That(
                httpRequest,
                Has.Property(nameof(HttpRequestMessage.RequestUri)).Property(nameof(Uri.AbsoluteUri)).EqualTo(m_Url)
            );

            AssertThatHasHeader(httpRequest, "Authorization", "Bearer " + accessToken);
            AssertThatHasHeader(httpRequest, "Accept", "application/json");

            var content = await httpRequest.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(content);

            Assert.That(jObject["Target"].Value<string>(), Is.EqualTo(m_HubMethodName));
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
