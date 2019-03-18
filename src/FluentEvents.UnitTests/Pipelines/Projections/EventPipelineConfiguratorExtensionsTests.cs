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
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;
        private SourceModel m_SourceModel;
        private SourceModelEventField m_SourceModelEventField;
        private Mock<IPipeline> m_PipelineMock;
        private EventPipelineConfigurator<TestSource, TestEventArgs> m_EventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
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
            m_SourceModelsServiceMock.Verify();
            m_PipelineMock.Verify();
        }

        [Test]
        public void ThenIsProjected_WithValidEventFieldName_ShouldAddPipelineModule(
            [Values] bool isEventFieldNameNull
        )
        {
            m_ServiceProviderMock
                .Setup(x => x.GetService(typeof(ISourceModelsService)))
                .Returns(m_SourceModelsServiceMock.Object)
                .Verifiable();

            var projectedSourceModel = new SourceModel(typeof(ProjectedTestSource));

            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(ProjectedTestSource)))
                .Returns(projectedSourceModel)
                .Verifiable();

            m_PipelineMock
                .Setup(x =>
                    x.AddModule<ProjectionPipelineModule, ProjectionPipelineModuleConfig>(
                        It.IsAny<ProjectionPipelineModuleConfig>()))
                .Verifiable();

            var newEventPipelineConfigurator = m_EventPipelineConfigurator.ThenIsProjected(
                x => new ProjectedTestSource(),
                x => new ProjectedTestEventArgs(),
                isEventFieldNameNull ? null : nameof(ProjectedTestSource.TestEvent2)
            );

            Assert.That(
                newEventPipelineConfigurator,
                Is.TypeOf<EventPipelineConfigurator<ProjectedTestSource, ProjectedTestEventArgs>>()
            );
            Assert.That(newEventPipelineConfigurator.Get<SourceModel>(), Is.EqualTo(projectedSourceModel));
            Assert.That(newEventPipelineConfigurator.Get<IServiceProvider>(), Is.EqualTo(m_ServiceProviderMock.Object));
            Assert.That(newEventPipelineConfigurator.Get<IPipeline>(), Is.EqualTo(m_PipelineMock.Object));
        }


        [Test]
        public void ThenIsProjected_WithInvalidEventFieldName_ShouldThrow()
        {
            m_ServiceProviderMock
                .Setup(x => x.GetService(typeof(ISourceModelsService)))
                .Returns(m_SourceModelsServiceMock.Object)
                .Verifiable();

            var projectedSourceModel = new SourceModel(typeof(ProjectedTestSource));

            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(ProjectedTestSource)))
                .Returns(projectedSourceModel)
                .Verifiable();

            Assert.That(() =>
            {
                m_EventPipelineConfigurator.ThenIsProjected(
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
                m_EventPipelineConfigurator.ThenIsProjected(
                    isSenderConverterNull ? (Func<TestSource, ProjectedTestSource>)null : x => new ProjectedTestSource(),
                    isEventArgsConverterNull ? (Func<TestEventArgs, ProjectedTestEventArgs>)null : x => new ProjectedTestEventArgs()
                );
            }, Throws.TypeOf<ArgumentNullException>());
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
