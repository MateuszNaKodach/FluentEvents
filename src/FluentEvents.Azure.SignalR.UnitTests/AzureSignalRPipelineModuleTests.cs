using System;
using System.Threading.Tasks;
using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Pipelines;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests
{
    [TestFixture]
    public class AzureSignalRPipelineModuleTests
    {
        private static readonly string[] ReceiverIds = {"1", "2"};

        private Mock<IAzureSignalRClient> m_AzureSignalRClientMock;
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<IServiceProvider> m_AppServiceProviderMock;
        private Mock<EventsContext> m_EventsContextMock;

        private AzureSignalRPipelineModuleConfig m_AzureSignalRPipelineModuleConfig;
        private EventsScope m_EventsScope;
        private PipelineEvent m_PipelineEvent;
        private PipelineContext m_PipelineContext;

        private AzureSignalRPipelineModule m_AzureSignalRPipelineModule;

        [SetUp]
        public void SetUp()
        {
            m_AzureSignalRClientMock = new Mock<IAzureSignalRClient>(MockBehavior.Strict);
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_AppServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_EventsContextMock = new Mock<EventsContext>(MockBehavior.Strict);

            m_AzureSignalRPipelineModuleConfig = new AzureSignalRPipelineModuleConfig
            {
                HubMethodName = nameof(AzureSignalRPipelineModuleConfig.HubMethodName),
                HubName = nameof(AzureSignalRPipelineModuleConfig.HubName),
                PublicationMethod = PublicationMethod.User,
                ReceiverIdsProviderAction = (o, o1) => ReceiverIds
            };
            m_EventsScope = new EventsScope(new []{m_EventsContextMock.Object}, m_AppServiceProviderMock.Object);
            m_PipelineEvent = new PipelineEvent(typeof(object), "", new object(), new object());
            m_PipelineContext = new PipelineContext(m_PipelineEvent, m_EventsScope, m_ServiceProviderMock.Object);

            m_AzureSignalRPipelineModule = new AzureSignalRPipelineModule(m_AzureSignalRClientMock.Object);
        }

        [Test]
        public async Task InvokeAsync_ShouldSendEventWithAzureSignalRClientAndInvokeNextModule(
            [Values] bool isReceiverIdsProviderActionNull
        )
        {
            if (isReceiverIdsProviderActionNull)
                m_AzureSignalRPipelineModuleConfig.ReceiverIdsProviderAction = null;

            m_AzureSignalRClientMock
                .Setup(x => x.SendEventAsync(
                    m_AzureSignalRPipelineModuleConfig.PublicationMethod,
                    m_AzureSignalRPipelineModuleConfig.HubName,
                    m_AzureSignalRPipelineModuleConfig.HubMethodName,
                    isReceiverIdsProviderActionNull ? null : ReceiverIds,
                    m_PipelineEvent.OriginalSender,
                    m_PipelineEvent.OriginalEventArgs)
                )
                .Returns(Task.CompletedTask)
                .Verifiable();

            var isNextModuleInvoked = false;

            await m_AzureSignalRPipelineModule.InvokeAsync(
                m_AzureSignalRPipelineModuleConfig,
                m_PipelineContext,
                context =>
                {
                    isNextModuleInvoked = true;
                    return Task.CompletedTask;
                }
            );

            Assert.That(isNextModuleInvoked, Is.True);
        }
    }
}
