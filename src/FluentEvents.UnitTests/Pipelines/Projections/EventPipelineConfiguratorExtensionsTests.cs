using System;
using FluentEvents.Config;
using FluentEvents.Model;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Projections;
using FluentEvents.Utils;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Projections
{
    [TestFixture]
    public class EventPipelineConfiguratorExtensionsTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<ISourceModelsService> _sourceModelsServiceMock;
        private Mock<IEventSelectionService> _eventSelectionServiceMock;
        private SourceModel _sourceModel;
        private SourceModel _projectedSourceModel;
        private SourceModelEventField _sourceModelEventField;
        private Mock<IPipeline> _pipelineMock;
        private EventPipelineConfigurator<TestSource, TestEventArgs> _eventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _eventSelectionServiceMock = new Mock<IEventSelectionService>(MockBehavior.Strict);
            _sourceModel = new SourceModel(typeof(TestSource));
            _projectedSourceModel = new SourceModel(typeof(ProjectedTestSource));
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
            _sourceModelsServiceMock.Verify();
            _eventSelectionServiceMock.Verify();
            _pipelineMock.Verify();
        }

        [Test]
        public void ThenIsProjected_WithValidEventFieldName_ShouldAddPipelineModule(
            [Values] bool isEventFieldNameNull
        )
        {
            SetUpServiceProvider();
            SetUpSourceModelsService();
            ProjectionPipelineModuleConfig config = null;
            SetUpPipeline(callbackConfig => config = callbackConfig);

            var newEventPipelineConfigurator = _eventPipelineConfigurator.ThenIsProjected(
                x => new ProjectedTestSource(),
                x => new ProjectedTestEventArgs(),
                isEventFieldNameNull ? null : nameof(ProjectedTestSource.TestEvent2)
            );

            var expectedEventFieldName = isEventFieldNameNull
                ? nameof(ProjectedTestSource.TestEvent)
                : nameof(ProjectedTestSource.TestEvent2);
            AssertThatPipelineModuleIsAdded(newEventPipelineConfigurator, config, expectedEventFieldName);
        }

        [Test]
        public void ThenIsProjected_WithSelectionAction_ShouldAddPipelineModule()
        {
            SetUpServiceProvider();
            SetUpSourceModelsService();
            ProjectionPipelineModuleConfig config = null;
            SetUpPipeline(callbackConfig => config = callbackConfig);

            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IEventSelectionService)))
                .Returns(_eventSelectionServiceMock.Object)
                .Verifiable();
            
            Action<ProjectedTestSource, dynamic> selectionAction = (x, y) => { };

            _eventSelectionServiceMock
                .Setup(x => x.GetSingleSelectedEventName(_projectedSourceModel, selectionAction))
                .Returns(nameof(ProjectedTestSource.TestEvent2))
                .Verifiable();

            var newEventPipelineConfigurator = _eventPipelineConfigurator.ThenIsProjected(
                x => new ProjectedTestSource(),
                x => new ProjectedTestEventArgs(),
                selectionAction
            );

            AssertThatPipelineModuleIsAdded(newEventPipelineConfigurator, config, nameof(ProjectedTestSource.TestEvent2));
        }

        [Test]
        public void ThenIsProjected_WithInvalidEventFieldName_ShouldThrow()
        {
            SetUpServiceProvider();
            SetUpSourceModelsService();

            Assert.That(() =>
            {
                _eventPipelineConfigurator.ThenIsProjected(
                    x => new ProjectedTestSource(),
                    x => new ProjectedTestEventArgs(),
                    "invalid"
                );
            }, Throws.TypeOf<EventFieldNotFoundException>());
        }

        [Test]
        [Sequential]
        public void ThenIsProjected_WithNullArgs_ShouldThrow(
            [Values(true, false)] bool isSenderConverterNull,
            [Values(false, true)] bool isEventArgsConverterNull
        )
        {
            Assert.That(() =>
            {
                _eventPipelineConfigurator.ThenIsProjected(
                    isSenderConverterNull ? (Func<TestSource, ProjectedTestSource>)null : x => new ProjectedTestSource(),
                    isEventArgsConverterNull ? (Func<TestEventArgs, ProjectedTestEventArgs>)null : x => new ProjectedTestEventArgs()
                );
            }, Throws.TypeOf<ArgumentNullException>());
        }

        private void SetUpServiceProvider()
        {
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(ISourceModelsService)))
                .Returns(_sourceModelsServiceMock.Object)
                .Verifiable();
        }

        private void AssertThatPipelineModuleIsAdded(
            EventPipelineConfigurator<ProjectedTestSource, ProjectedTestEventArgs> newEventPipelineConfigurator,
            ProjectionPipelineModuleConfig config,
            string expectedEventFieldName
        )
        {
            Assert.That(
                newEventPipelineConfigurator,
                Is.TypeOf<EventPipelineConfigurator<ProjectedTestSource, ProjectedTestEventArgs>>()
            );
            Assert.That(
                config, 
                Has
                    .Property(nameof(ProjectionPipelineModuleConfig.ProjectedEventFieldName))
                    .EqualTo(expectedEventFieldName)
            );
            Assert.That(newEventPipelineConfigurator.Get<SourceModel>(), Is.EqualTo(_projectedSourceModel));
            Assert.That(newEventPipelineConfigurator.Get<IServiceProvider>(), Is.EqualTo(_serviceProviderMock.Object));
            Assert.That(newEventPipelineConfigurator.Get<IPipeline>(), Is.EqualTo(_pipelineMock.Object));
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

        private void SetUpSourceModelsService()
        {
            _sourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(ProjectedTestSource)))
                .Returns(_projectedSourceModel)
                .Verifiable();
        }

        private class TestSource
        {
            public event EventHandler<TestEventArgs> TestEvent;
        }

        private class TestEventArgs
        {

        }

        private class ProjectedTestSource
        {
            public event EventHandler<ProjectedTestEventArgs> TestEvent;
            public event EventHandler<ProjectedTestEventArgs> TestEvent2;
        }

        private class ProjectedTestEventArgs
        {

        }
    }
}
