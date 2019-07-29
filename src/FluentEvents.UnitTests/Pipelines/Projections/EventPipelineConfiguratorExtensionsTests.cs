using System;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Projections;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Projections
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
        public void ThenIsProjected_ShouldAddPipelineModule()
        {
            ProjectionPipelineModuleConfig config = null;
            SetUpPipeline(callbackConfig => config = callbackConfig);

            var newEventPipelineConfigurator = _eventPipelineConfiguration.ThenIsProjected(x => new ProjectedEvent());
            
            Assert.That(
                newEventPipelineConfigurator,
                Is.TypeOf<EventPipelineConfiguration<ProjectedEvent>>()
            );
            
            Assert.That(config, Has.Property(nameof(ProjectionPipelineModuleConfig.EventProjection)).Not.Null);
            Assert.That(newEventPipelineConfigurator.Get<IServiceProvider>(), Is.EqualTo(_serviceProviderMock.Object));
            Assert.That(newEventPipelineConfigurator.Get<IPipeline>(), Is.EqualTo(_pipelineMock.Object));
        }
        
        [Test]
        public void ThenIsProjected_WithNullArgs_ShouldThrow()
        {
            Assert.That(() =>
            {
                _eventPipelineConfiguration.ThenIsProjected<object, object>(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        private void SetUpPipeline(Action<ProjectionPipelineModuleConfig> callback)
        {
            _pipelineMock
                .Setup(x =>
                    x.AddModule<ProjectionPipelineModule, ProjectionPipelineModuleConfig>(
                        It.IsAny<ProjectionPipelineModuleConfig>()
                    )
                )
                .Callback(callback)
                .Verifiable();
        }
        
        private class ProjectedEvent
        {
        }
    }
}
