using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Azure.SignalR.Client;
using FluentEvents.IntegrationTests.Common;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.IntegrationTests
{
    public class PublishToAzureSignalRTestBase<TEventsContext> where TEventsContext : EventsContext
    {
        protected static readonly string UserId1 = Guid.NewGuid().ToString();
        protected static readonly string UserId2 = Guid.NewGuid().ToString();
        protected static readonly string UserId3 = Guid.NewGuid().ToString();
        private int m_ReceivedEventsCount;

        protected const string HubName = "testHub";
        protected const string HubMethodName = "testHubMethod";

        private protected ConnectionString ConnectionString { get; private set; }

        protected IServiceProvider Provider { get; private set; }
        protected EventsScope Scope { get; private set; }
        protected HubConnection HubConnection1 { get; private set; }
        protected HubConnection HubConnection2 { get; private set; }
        protected HubConnection HubConnection3 { get; private set; }

        protected TEventsContext EventsContext { get; private set; }

        protected int ReceivedEventsCount
        {
            get => m_ReceivedEventsCount;
        }

        [SetUp]
        public async Task SetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<PublishToAllAzureSignalRUsersTest>()
                .Build();

            ConnectionString = configuration["azureSignalRService:connectionString"];

            if (string.IsNullOrEmpty(ConnectionString))
                Assert.Ignore("Azure SignalR Service settings not found in user secrets.");

            var hubUrl = $"{ConnectionString.Endpoint.TrimEnd('/')}/client/?hub={HubName}";

            var services = new ServiceCollection();

            AddEventsContext<TEventsContext>(services, configuration);

            Provider = services.BuildServiceProvider();

            EventsContext = Provider.GetRequiredService<TEventsContext>();

            Scope = Provider.CreateScope().ServiceProvider.GetService<EventsScope>();

            HubConnection1 = await SetUpHubConnection(ConnectionString, hubUrl, UserId1);
            HubConnection2 = await SetUpHubConnection(ConnectionString, hubUrl, UserId2);
            HubConnection3 = await SetUpHubConnection(ConnectionString, hubUrl, UserId3);

            m_ReceivedEventsCount = 0;
        }

        private static void AddEventsContext<TContext>(IServiceCollection services, IConfiguration configuration)
            where TContext : EventsContext
        {
            services.AddEventsContext<TContext>(options =>
            {
                options.UseAzureSignalRService(configuration.GetSection("azureSignalRService"));
            });
        }

        private async Task<HubConnection> SetUpHubConnection(string connectionString, string hubUrl, string nameIdentifier)
        {
            var accessTokensService = new AccessTokensService();
            var accessToken = accessTokensService.GenerateAccessToken(connectionString, hubUrl, nameIdentifier);

            var hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(accessToken);
                })
                .Build();

            await hubConnection.StartAsync();

            return hubConnection;
        }

        protected async Task CheckEventPublishing(
            HubConnection hubConnection,
            SemaphoreSlim semaphoreSlim,
            bool isPublishingExpected
        )
        {
            TestEntity receivedSender = null;
            TestEventArgs receivedEventArgs = null;
            hubConnection.On<TestEntity, TestEventArgs>(HubMethodName, (sender, eventArgs) =>
            {
                receivedSender = sender;
                receivedEventArgs = eventArgs;
            });

            await semaphoreSlim.WaitAsync();

            await Watcher.WaitUntilAsync(() => receivedEventArgs != null, 6000);

            if (receivedEventArgs != null)
                Interlocked.Increment(ref m_ReceivedEventsCount);

            if (isPublishingExpected)
            {
                TestUtils.AssertThatEventIsPublishedProperly(receivedSender, receivedEventArgs);
            }
            else
            {
                Assert.That(receivedSender, Is.Null);
                Assert.That(receivedEventArgs, Is.Null);
            }
        }
    }
}