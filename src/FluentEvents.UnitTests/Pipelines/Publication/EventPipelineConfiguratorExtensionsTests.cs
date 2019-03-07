using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Transmission;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Publication
{
    [TestFixture]
    public class EventPipelineConfiguratorExtensionsTests
    {
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private SourceModel m_SourceModel;
        private SourceModelEventField m_SourceModelEventField;
        private Mock<IPipeline> m_PipelineMock;
        private EventPipelineConfigurator<TestSource, TestEventArgs> m_EventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
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
            m_PipelineMock.Verify();
        }

        [Test]
        public void ThenIsPublishedToScopedSubscriptions_ShouldAddPipelineModule()
        {
            m_PipelineMock
                .Setup(x => x.AddModule<ScopedPublishPipelineModule, ScopedPublishPipelineModuleConfig>(
                        It.IsAny<ScopedPublishPipelineModuleConfig>()
                    )
                )
                .Verifiable();

            var eventPipelineConfigurator = m_EventPipelineConfigurator.ThenIsPublishedToScopedSubscriptions();
            
            Assert.That(eventPipelineConfigurator, Is.EqualTo(m_EventPipelineConfigurator));
        }

        [Test]
        public void ThenIsPublishedToGlobalSubscriptions_WithoutArgs_ShouldAddPipelineModuleWithoutEventSender()
        {
            m_PipelineMock
                .Setup(x => x.AddModule<GlobalPublishPipelineModule, GlobalPublishPipelineModuleConfig>(
                        It.Is<GlobalPublishPipelineModuleConfig>(y => y.SenderType == null)
                    )
                )
                .Verifiable();

            var eventPipelineConfigurator = m_EventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions();

            Assert.That(eventPipelineConfigurator, Is.EqualTo(m_EventPipelineConfigurator));
        }

        [Test]
        public void ThenIsPublishedToGlobalSubscriptions_WithTransmissionConfiguration_ShouldAddPipelineModuleWithEventSender()
        {
            m_ServiceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<IEventSender>)))
                .Returns(new [] { new TestEventSender() })
                .Verifiable();

            m_PipelineMock
                .Setup(x => x.AddModule<GlobalPublishPipelineModule, GlobalPublishPipelineModuleConfig>(
                        It.Is<GlobalPublishPipelineModuleConfig>(y => y.SenderType == typeof(TestEventSender))
                    )
                )
                .Verifiable();

            var eventPipelineConfigurator = m_EventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions(
                x => ((IConfigureTransmission)x).With<TestEventSender>()
            );

            Assert.That(eventPipelineConfigurator, Is.EqualTo(m_EventPipelineConfigurator));
        }


        [Test]
        public void ThenIsPublishedToGlobalSubscriptions_WithTransmissionConfigurationAndEventSenderNotRegistered_ShouldThrow()
        {
            m_ServiceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<IEventSender>)))
                .Returns(new IEventSender[0])
                .Verifiable();

            Assert.That(() =>
            {
                m_EventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions(
                    x => ((IConfigureTransmission)x).With<TestEventSender>()
                );
            }, Throws.TypeOf<EventTransmissionPluginIsNotConfiguredException>());
            
        }

        private class TestEventSender : IEventSender
        {
            public Task SendAsync(PipelineEvent pipelineEvent) => throw new NotImplementedException();
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
