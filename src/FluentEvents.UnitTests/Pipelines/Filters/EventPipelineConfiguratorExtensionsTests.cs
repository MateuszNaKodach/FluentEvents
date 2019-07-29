using System;
using FluentEvents.Configuration;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Filters;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Filters
{
    [TestFixture]
    public class EventPipelineConfiguratorExtensionsTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<ISourceModelsService> _sourceModelsServiceMock;
        private Mock<IPipeline> _pipelineMock;

        private EventPipelineConfiguration<object> _eventPipelineConfiguration;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);

            _eventPipelineConfiguration = new EventPipelineConfiguration<object>(
                _serviceProviderMock.Object,
                _pipelineMock.Object
            );
        }

        [Test]
        public void ThenIsFiltered_ShouldAddPipelineModule([Values] bool isMatching)
        {
            FilterPipelineModuleConfig config = null;
            _pipelineMock
                .Setup(x =>
                    x.AddModule<FilterPipelineModule, FilterPipelineModuleConfig>(It.IsAny<FilterPipelineModuleConfig>())
                )
                .Callback<FilterPipelineModuleConfig>(paramsConfig => config = paramsConfig)
                .Verifiable();

            var eventPipelineConfigurator = _eventPipelineConfiguration.ThenIsFiltered(e => isMatching);

            Assert.That(eventPipelineConfigurator, Is.EqualTo(_eventPipelineConfiguration));
            Assert.That(config, Is.Not.Null);
            Assert.That(config.IsMatching(new object()), Is.EqualTo(isMatching));
        }

        [Test]
        public void ThenIsFiltered_WithNullFilter_ShouldThrow()
        {
            Assert.That(() =>
            {
                _eventPipelineConfiguration.ThenIsFiltered(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProviderMock.Verify();
            _sourceModelsServiceMock.Verify();
            _pipelineMock.Verify();
        }
    }
}
