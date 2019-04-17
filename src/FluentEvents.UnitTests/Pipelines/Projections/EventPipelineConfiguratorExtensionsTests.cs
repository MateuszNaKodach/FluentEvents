using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;
        private Mock<IEventSelectionService> m_EventSelectionServiceMock;
        private SourceModel m_SourceModel;
        private SourceModel m_ProjectedSourceModel;
        private SourceModelEventField m_SourceModelEventField;
        private Mock<IPipeline> m_PipelineMock;
        private EventPipelineConfigurator<TestSource, TestEventArgs> m_EventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            m_EventSelectionServiceMock = new Mock<IEventSelectionService>(MockBehavior.Strict);
            m_SourceModel = new SourceModel(typeof(TestSource));
            m_ProjectedSourceModel = new SourceModel(typeof(ProjectedTestSource));
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
            SetUpServiceProvider();
            SetUpSourceModelsService();
            SetUpPipeline();

            var newEventPipelineConfigurator = m_EventPipelineConfigurator.ThenIsProjected(
                x => new ProjectedTestSource(),
                x => new ProjectedTestEventArgs(),
                isEventFieldNameNull ? null : nameof(ProjectedTestSource.TestEvent2)
            );

            AssertThatPipelineModuleIsAdded(newEventPipelineConfigurator);
        }

        [Test]
        public void ThenIsProjected_WithSelectionAction_ShouldAddPipelineModule()
        {
            SetUpServiceProvider();
            SetUpSourceModelsService();
            SetUpPipeline();

            m_ServiceProviderMock
                .Setup(x => x.GetService(typeof(IEventSelectionService)))
                .Returns(m_EventSelectionServiceMock.Object)
                .Verifiable();
            
            Action<ProjectedTestSource, dynamic> selectionAction = (x, y) => { };

            m_EventSelectionServiceMock
                .Setup(x => x.GetSelectedEvent(m_ProjectedSourceModel, selectionAction))
                .Returns(new [] { nameof(ProjectedTestSource.TestEvent2) })
                .Verifiable();

            var newEventPipelineConfigurator = m_EventPipelineConfigurator.ThenIsProjected(
                x => new ProjectedTestSource(),
                x => new ProjectedTestEventArgs(),
                selectionAction
            );

            AssertThatPipelineModuleIsAdded(newEventPipelineConfigurator);
        }

        [Test]
        public void ThenIsProjected_WithInvalidEventFieldName_ShouldThrow()
        {
            SetUpServiceProvider();
            SetUpSourceModelsService();

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

        private void SetUpServiceProvider()
        {
            m_ServiceProviderMock
                .Setup(x => x.GetService(typeof(ISourceModelsService)))
                .Returns(m_SourceModelsServiceMock.Object)
                .Verifiable();
        }

        private void AssertThatPipelineModuleIsAdded(
            EventPipelineConfigurator<ProjectedTestSource, ProjectedTestEventArgs> newEventPipelineConfigurator
        )
        {
            Assert.That(
                newEventPipelineConfigurator,
                Is.TypeOf<EventPipelineConfigurator<ProjectedTestSource, ProjectedTestEventArgs>>()
            );
            Assert.That(newEventPipelineConfigurator.Get<SourceModel>(), Is.EqualTo(m_ProjectedSourceModel));
            Assert.That(newEventPipelineConfigurator.Get<IServiceProvider>(), Is.EqualTo(m_ServiceProviderMock.Object));
            Assert.That(newEventPipelineConfigurator.Get<IPipeline>(), Is.EqualTo(m_PipelineMock.Object));
        }

        private void SetUpPipeline()
        {
            m_PipelineMock
                .Setup(x =>
                    x.AddModule<ProjectionPipelineModule, ProjectionPipelineModuleConfig>(
                        It.IsAny<ProjectionPipelineModuleConfig>()
                    )
                )
                .Verifiable();
        }

        private void SetUpSourceModelsService()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(ProjectedTestSource)))
                .Returns(m_ProjectedSourceModel)
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
