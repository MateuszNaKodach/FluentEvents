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
        private Mock<IServiceProvider> _serviceProviderMock;
        private SourceModel _sourceModel;
        private SourceModelEventField _sourceModelEventField;
        private Mock<IPipeline> _pipelineMock;
        private EventPipelineConfigurator<TestSource, TestEventArgs> _eventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _sourceModel = new SourceModel(typeof(TestSource));
            _sourceModelEventField = _sourceModel.GetOrCreateEventField(nameof(TestSource.TestEvent));
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
       
            _eventPipelineConfigurator = new EventPipelineConfigurator<TestSource, TestEventArgs>(
                _sourceModel,
                _sourceModelEventField,
                _serviceProviderMock.Object,
                _pipelineMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProviderMock.Verify();
            _pipelineMock.Verify();
        }

        [Test]
        public void ThenIsPublishedToScopedSubscriptions_ShouldAddPipelineModule()
        {
            _pipelineMock
                .Setup(x => x.AddModule<ScopedPublishPipelineModule, ScopedPublishPipelineModuleConfig>(
                        It.IsAny<ScopedPublishPipelineModuleConfig>()
                    )
                )
                .Verifiable();

            var eventPipelineConfigurator = _eventPipelineConfigurator.ThenIsPublishedToScopedSubscriptions();
            
            Assert.That(eventPipelineConfigurator, Is.EqualTo(_eventPipelineConfigurator));
        }

        [Test]
        public void ThenIsPublishedToGlobalSubscriptions_WithoutArgs_ShouldAddPipelineModuleWithoutEventSender()
        {
            _pipelineMock
                .Setup(x => x.AddModule<GlobalPublishPipelineModule, GlobalPublishPipelineModuleConfig>(
                        It.Is<GlobalPublishPipelineModuleConfig>(y => y.SenderType == null)
                    )
                )
                .Verifiable();

            var eventPipelineConfigurator = _eventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions();

            Assert.That(eventPipelineConfigurator, Is.EqualTo(_eventPipelineConfigurator));
        }

        [Test]
        public void ThenIsPublishedToGlobalSubscriptions_WithNullArgs_ShouldThrow()
        {
            Assert.That(() =>
            {
                _eventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ThenIsPublishedToGlobalSubscriptions_WithTransmissionConfiguration_ShouldAddPipelineModuleWithEventSender()
        {
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<IEventSender>)))
                .Returns(new [] { new TestEventSender() })
                .Verifiable();

            _pipelineMock
                .Setup(x => x.AddModule<GlobalPublishPipelineModule, GlobalPublishPipelineModuleConfig>(
                        It.Is<GlobalPublishPipelineModuleConfig>(y => y.SenderType == typeof(TestEventSender))
                    )
                )
                .Verifiable();

            var eventPipelineConfigurator = _eventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions(
                x => ((IConfigureTransmission)x).With<TestEventSender>()
            );

            Assert.That(eventPipelineConfigurator, Is.EqualTo(_eventPipelineConfigurator));
        }


        [Test]
        public void ThenIsPublishedToGlobalSubscriptions_WithTransmissionConfigurationAndEventSenderNotRegistered_ShouldThrow()
        {
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<IEventSender>)))
                .Returns(new IEventSender[0])
                .Verifiable();

            Assert.That(() =>
            {
                _eventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions(
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
