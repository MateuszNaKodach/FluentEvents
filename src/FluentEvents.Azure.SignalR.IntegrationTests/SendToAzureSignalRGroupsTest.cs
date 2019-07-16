using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.IntegrationTests.Common;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.IntegrationTests
{
    [TestFixture]
    public class SendToAzureSignalRGroupsTest 
        : SendToAzureSignalRTestBase<SendToAzureSignalRGroupsTest.TestEventsContext>
    {
        private static readonly string _groupId1 = Guid.NewGuid().ToString();
        private static readonly string _groupId2 = Guid.NewGuid().ToString();

        [SetUp]
        public async Task SetUpGroups()
        {
            await SetUpGroupAsync(UserId1, _groupId1);
            await SetUpGroupAsync(UserId2, _groupId1);
            await SetUpGroupAsync(UserId3, _groupId2);
        }

        [Test]
        public async Task Test()
        {
            var semaphoreSlim = new SemaphoreSlim(3);
            var task1 = CheckEventPublishing(HubConnection1, semaphoreSlim, true);
            var task2 = CheckEventPublishing(HubConnection2, semaphoreSlim, true);
            var task3 = CheckEventPublishing(HubConnection3, semaphoreSlim, false);
            var allTasks = Task.WhenAll(task1, task2, task3);

            TestUtils.AttachAndRaiseEvent(EventsContext);
            semaphoreSlim.Release(3);

            await allTasks;

            Assert.That(ReceivedEventsCount, Is.EqualTo(2));
        }

        private async Task SetUpGroupAsync(string userId, string groupId)
        {
            var endpoint = ConnectionString.Endpoint;
            var url = $"{endpoint.TrimEnd('/')}/api/v1/hubs/{HubName}/groups/{groupId}/users/{userId}";

            var accessTokensService = new AccessTokensService();
            var accessToken = accessTokensService.GenerateAccessToken(ConnectionString, url);

            var httpClient = new HttpClient();
            var mimeType = "application/json";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mimeType));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.PutAsync(url, new StringContent("{}", Encoding.UTF8, mimeType));
            response.EnsureSuccessStatusCode();
        }

        public class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped()
                    .ThenIsSentToAzureSignalRGroups(e => new[] { _groupId1 }, HubName, HubMethodName);
            }

            public TestEventsContext(
                EventsContextOptions options,
                IAppServiceProvider appServiceProvider,
                IScopedAppServiceProvider scopedAppServiceProvider
            ) : base(options, appServiceProvider, scopedAppServiceProvider)
            {
            }
        }
    }
}
