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
        private static readonly string[] _receiverIds = {"1", "2"};

        private Mock<IEventSendingService> _azureSignalRClientMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IServiceProvider> _appServiceProviderMock;
        private Mock<EventsContext> _eventsContextMock;

        private AzureSignalRPipelineModuleConfig _azureSignalRPipelineModuleConfig;
        private EventsScope _eventsScope;
        private PipelineEvent _pipelineEvent;
        private PipelineContext _pipelineContext;

        private AzureSignalRPipelineModule _azureSignalRPipelineModule;

        [SetUp]
        public void SetUp()
        {
            _azureSignalRClientMock = new Mock<IEventSendingService>(MockBehavior.Strict);
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _appServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _eventsContextMock = new Mock<EventsContext>(MockBehavior.Strict);

            _azureSignalRPipelineModuleConfig = new AzureSignalRPipelineModuleConfig
            {
                HubMethodName = nameof(AzureSignalRPipelineModuleConfig.HubMethodName),
                HubName = nameof(AzureSignalRPipelineModuleConfig.HubName),
                PublicationMethod = PublicationMethod.User,
                ReceiverIdsProviderAction = e => _receiverIds
            };
            _eventsScope = new EventsScope(new[] {_eventsContextMock.Object}, _appServiceProviderMock.Object);
            _pipelineEvent = new PipelineEvent(typeof(object));
            _pipelineContext = new PipelineContext(_pipelineEvent, _eventsScope, _serviceProviderMock.Object);

            _azureSignalRPipelineModule = new AzureSignalRPipelineModule(_azureSignalRClientMock.Object);
        }

        [Test]
        public async Task InvokeAsync_ShouldSendEventWithAzureSignalRClientAndInvokeNextModule(
            [Values] bool isReceiverIdsProviderActionNull
        )
        {
            if (isReceiverIdsProviderActionNull)
                _azureSignalRPipelineModuleConfig.ReceiverIdsProviderAction = null;

            _azureSignalRClientMock
                .Setup(x => x.SendEventAsync(
                    _azureSignalRPipelineModuleConfig.PublicationMethod,
                    _azureSignalRPipelineModuleConfig.HubName,
                    _azureSignalRPipelineModuleConfig.HubMethodName,
                    isReceiverIdsProviderActionNull ? null : _receiverIds,
                    _pipelineEvent.Event)
                )
                .Returns(Task.CompletedTask)
                .Verifiable();

            var isNextModuleInvoked = false;

            await _azureSignalRPipelineModule.InvokeAsync(
                _azureSignalRPipelineModuleConfig,
                _pipelineContext,
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
