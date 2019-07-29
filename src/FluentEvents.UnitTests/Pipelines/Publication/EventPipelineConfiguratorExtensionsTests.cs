using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Configuration;
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
        private Mock<IPipeline> _pipelineMock;

        private EventPipelineConfiguration<object> _eventPipelineConfiguration;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
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

            var eventPipelineConfigurator = _eventPipelineConfiguration.ThenIsPublishedToScopedSubscriptions();
            
            Assert.That(eventPipelineConfigurator, Is.EqualTo(_eventPipelineConfiguration));
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

            var eventPipelineConfigurator = _eventPipelineConfiguration.ThenIsPublishedToGlobalSubscriptions();

            Assert.That(eventPipelineConfigurator, Is.EqualTo(_eventPipelineConfiguration));
        }

        [Test]
        public void ThenIsPublishedToGlobalSubscriptions_WithNullArgs_ShouldThrow()
        {
            Assert.That(() =>
            {
                _eventPipelineConfiguration.ThenIsPublishedToGlobalSubscriptions(null);
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

            var eventPipelineConfigurator = _eventPipelineConfiguration.ThenIsPublishedToGlobalSubscriptions(
                x => ((IConfigureTransmission)x).With<TestEventSender>()
            );

            Assert.That(eventPipelineConfigurator, Is.EqualTo(_eventPipelineConfiguration));
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
                _eventPipelineConfiguration.ThenIsPublishedToGlobalSubscriptions(
                    x => ((IConfigureTransmission)x).With<TestEventSender>()
                );
            }, Throws.TypeOf<EventTransmissionPluginIsNotConfiguredException>());
            
        }

        private class TestEventSender : IEventSender
        {
            public Task SendAsync(PipelineEvent pipelineEvent) => throw new NotImplementedException();
        }
    }
}
