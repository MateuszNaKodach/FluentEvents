using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using FluentEvents.Azure.SignalR.Client;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests.Clients
{
    [TestFixture]
    public class AccessTokensServiceTests
    {
        private const string Audience = nameof(Audience);
        private readonly ConnectionString m_ConnectionStringWithValidAccessKey = "Endpoint=123;AccessKey=tBNrNxHRYlTuqAWUUb3Y0YMHgY7iCd9HtA7nCsfANB8;";
        private readonly ConnectionString m_ConnectionStringWithInvalidAccessKey = "Endpoint=123;AccessKey=123;";

        private AccessTokensService m_AccessTokensService;

        [SetUp]
        public void SetUp()
        {
            m_AccessTokensService = new AccessTokensService();
        }

        [Test]
        public void GenerateAccessToken_WithValidAccessKey_ShouldReturnValidJwt()
        {
            var accessToken = m_AccessTokensService.GenerateAccessToken(m_ConnectionStringWithValidAccessKey, Audience);

            Assert.That(accessToken, Is.Not.Null);
            Assert.That(accessToken, Has.Exactly(2).Items.EqualTo('.'));

            var encodedJson = accessToken.Split('.', StringSplitOptions.RemoveEmptyEntries)[1];

            encodedJson = encodedJson.PadRight(4 * ((encodedJson.Length + 3) / 4), '=');

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(encodedJson));
            var jObject = JObject.Parse(json);

            Assert.That(jObject["nameid"], Is.Not.Null);
            Assert.That(jObject["nbf"], Is.Not.Null);
            Assert.That(jObject["exp"], Is.Not.Null);
            Assert.That(jObject["iat"], Is.Not.Null);
            Assert.That(jObject["aud"].Value<string>(), Is.EqualTo(Audience));
        }

        [Test]
        public void GenerateAccessToken_WithInvalidAccessKey_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_AccessTokensService.GenerateAccessToken(m_ConnectionStringWithInvalidAccessKey, Audience);
            }, Throws.TypeOf<InvalidConnectionStringAccessKeyException>());
        }
    }
}
