using System;
using System.Threading.Tasks;
using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.IntegrationTests
{
    [TestFixture]
    public class PublishToAzureSignalRTest
    {
        private const string HubName = "testHub";
        private const string HubMethodName = "testHubMethod";

        private TestEventsContext m_TestEventsContext;
        private IServiceProvider m_ServiceProvider;
        private EventsScope m_EventsScope;
        private HubConnection m_HubConnection;

        [SetUp]
        public async Task SetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<PublishToAzureSignalRTest>()
                .Build();

            var connectionString = configuration["azureSignalRService:connectionString"];

            if (string.IsNullOrEmpty(connectionString))
                Assert.Ignore("Azure SignalR Service settings not found in user secrets.");

            var hubUrl = $"{configuration["azureSignalRService:hubUrl"]}client/?hub={HubName}";

            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options =>
            {
                options.UseAzureSignalRService(configuration.GetSection("azureSignalRService"));
            });

            m_ServiceProvider = services.BuildServiceProvider();

            m_TestEventsContext = m_ServiceProvider.GetRequiredService<TestEventsContext>();
            m_EventsScope = m_ServiceProvider.CreateScope().ServiceProvider.GetService<EventsScope>();

            await SetUpHubConnection(connectionString, hubUrl);
        }

        private async Task SetUpHubConnection(string connectionString, string hubUrl)
        {
            var accessTokensService = new AccessTokensService();
            var accessToken = accessTokensService.GenerateAccessToken(connectionString, hubUrl);

            m_HubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options => { options.AccessTokenProvider = () => Task.FromResult(accessToken); })
                .Build();

            await m_HubConnection.StartAsync();
        }

        [Test]
        public async Task Works()
        {
            TestEntity receivedSender = null;
            TestEventArgs receivedEventArgs = null;
            m_HubConnection.On<TestEntity, TestEventArgs>(HubMethodName, (sender, eventArgs) =>
            {
                receivedSender = sender;
                receivedEventArgs = eventArgs;
            });

            TestUtils.AttachAndRaiseEvent(m_TestEventsContext, m_EventsScope);

            await Watcher.WaitUntilAsync(() => receivedEventArgs != null);

            TestUtils.AssertThatEventIsPublishedProperly(receivedSender, receivedEventArgs);
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .IsForwardedToPipeline()
                    .ThenIsPublishedToAllAzureSignalRUsers(HubName, HubMethodName);
            }
        }
    }
}
