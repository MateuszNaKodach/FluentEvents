using System;
using FluentEvents.Config;
using FluentEvents.Model;
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

        private EventPipelineConfigurator<object> _eventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);

            _eventPipelineConfigurator = new EventPipelineConfigurator<object>(
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
        public void ThenIsProjected_ShouldAddPipelineModule(
            [Values] bool isEventFieldNameNull
        )
        {
            ProjectionPipelineModuleConfig config = null;
            SetUpPipeline(callbackConfig => config = callbackConfig);

            var newEventPipelineConfigurator = _eventPipelineConfigurator.ThenIsProjected(x => new ProjectedEvent());
            
            Assert.That(
                newEventPipelineConfigurator,
                Is.TypeOf<EventPipelineConfigurator<ProjectedEvent>>()
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
                _eventPipelineConfigurator.ThenIsProjected<object, object>(null);
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
