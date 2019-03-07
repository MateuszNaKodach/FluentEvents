using System;
using FluentEvents.Config;
using FluentEvents.Model;
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

        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<IEventsQueueNamesService> m_EventsQueueNamesServiceMock;
        private SourceModel m_SourceModel;
        private SourceModelEventField m_SourceModelEventField;
        private Mock<IPipeline> m_PipelineMock;

        private EventPipelineConfigurator<TestSource, TestEventArgs> m_EventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_EventsQueueNamesServiceMock = new Mock<IEventsQueueNamesService>(MockBehavior.Strict);
            m_SourceModel = new SourceModel(typeof(TestSource));
            m_SourceModelEventField = m_SourceModel.GetOrCreateEventField(nameof(TestSource.TestEvent));
            m_PipelineMock = new Mock<IPipeline>(MockBehavior.Strict);

            m_EventPipelineConfigurator = new EventPipelineConfigurator<TestSource, TestEventArgs>(
                m_SourceModel,
                m_SourceModelEventField,
                m_ServiceProviderMock.Object,
                m_PipelineMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_ServiceProviderMock.Verify();
            m_EventsQueueNamesServiceMock.Verify();
            m_PipelineMock.Verify();
        }

        [Test]
        public void ThenIsQueuedTo_ShouldAddPipelineModule()
        {
            m_PipelineMock
                .Setup(x => x.AddModule<EnqueuePipelineModule, EnqueuePipelineModuleConfig>(
                        It.Is<EnqueuePipelineModuleConfig>(y => y.QueueName == QueueName)
                    )
                )
                .Verifiable();

            m_ServiceProviderMock
                .Setup(x => x.GetService(typeof(IEventsQueueNamesService)))
                .Returns(m_EventsQueueNamesServiceMock.Object)
                .Verifiable();

            m_EventsQueueNamesServiceMock
                .Setup(x => x.RegisterQueueNameIfNotExists(QueueName))
                .Verifiable();

            m_EventPipelineConfigurator.ThenIsQueuedTo(QueueName);
        }

        [Test]
        public void ThenIsQueuedTo_WithNullQueueName_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_EventPipelineConfigurator.ThenIsQueuedTo(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        private class TestSource
        {
            public event EventHandler<TestEventArgs> TestEvent;
        }

        private class TestEventArgs
        {

        }
    }
}