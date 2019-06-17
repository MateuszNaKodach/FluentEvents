using System;
using FluentEvents.Config;
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

        private SourceModel _sourceModel;
        private SourceModelEventField _sourceModelEventField;

        private EventPipelineConfigurator<TestSource, TestEventArgs> _eventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);

            _sourceModel = new SourceModel(typeof(TestSource));
            _sourceModelEventField = _sourceModel.GetOrCreateEventField(nameof(TestSource.TestEvent));

            _eventPipelineConfigurator = new EventPipelineConfigurator<TestSource, TestEventArgs>(
                _sourceModel,
                _sourceModelEventField,
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

            var eventPipelineConfigurator = _eventPipelineConfigurator.ThenIsFiltered((x, y) => isMatching);

            Assert.That(eventPipelineConfigurator, Is.EqualTo(_eventPipelineConfigurator));
            Assert.That(config, Is.Not.Null);
            Assert.That(config.IsMatching(new TestSource(), new TestEventArgs()), Is.EqualTo(isMatching));
        }

        [Test]
        public void ThenIsFiltered_WithNullFilter_ShouldThrow()
        {
            Assert.That(() =>
            {
                _eventPipelineConfigurator.ThenIsFiltered(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProviderMock.Verify();
            _sourceModelsServiceMock.Verify();
            _pipelineMock.Verify();
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
