using System;
using FluentEvents.Configuration;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Queues;
using FluentEvents.Queues;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Queues
{
    [TestFixture]
    public class EventPipelineConfiguratorExtensionsTests
    {
        private const string QueueName = nameof(QueueName);

        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IEventsQueueNamesService> _eventsQueueNamesServiceMock;
        private Mock<IPipeline> _pipelineMock;

        private EventPipelineConfiguration<object> _eventPipelineConfiguration;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _eventsQueueNamesServiceMock = new Mock<IEventsQueueNamesService>(MockBehavior.Strict);
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);

            _eventPipelineConfiguration = new EventPipelineConfiguration<object>(
                _serviceProviderMock.Object,
                _pipelineMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProviderMock.Verify();
            _eventsQueueNamesServiceMock.Verify();
            _pipelineMock.Verify();
        }

        [Test]
        public void ThenIsQueuedTo_ShouldAddPipelineModule()
        {
            _pipelineMock
                .Setup(x => x.AddModule<EnqueuePipelineModule, EnqueuePipelineModuleConfig>(
                        It.Is<EnqueuePipelineModuleConfig>(y => y.QueueName == QueueName)
                    )
                )
                .Verifiable();

            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IEventsQueueNamesService)))
                .Returns(_eventsQueueNamesServiceMock.Object)
                .Verifiable();

            _eventsQueueNamesServiceMock
                .Setup(x => x.RegisterQueueNameIfNotExists(QueueName))
                .Verifiable();

            _eventPipelineConfiguration.ThenIsQueuedTo(QueueName);
        }

        [Test]
        public void ThenIsQueuedTo_WithNullQueueName_ShouldThrow()
        {
            Assert.That(() =>
            {
                _eventPipelineConfiguration.ThenIsQueuedTo(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}