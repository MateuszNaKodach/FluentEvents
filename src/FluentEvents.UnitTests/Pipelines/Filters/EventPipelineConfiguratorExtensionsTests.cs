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
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;
        private Mock<IPipeline> m_PipelineMock;

        private SourceModel m_SourceModel;
        private SourceModelEventField m_SourceModelEventField;

        private EventPipelineConfigurator<TestSource, TestEventArgs> m_EventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            m_PipelineMock = new Mock<IPipeline>(MockBehavior.Strict);

            m_SourceModel = new SourceModel(typeof(TestSource));
            m_SourceModelEventField = m_SourceModel.GetOrCreateEventField(nameof(TestSource.TestEvent));

            m_EventPipelineConfigurator = new EventPipelineConfigurator<TestSource, TestEventArgs>(
                m_SourceModel,
                m_SourceModelEventField,
                m_ServiceProviderMock.Object,
                m_PipelineMock.Object
            );
        }

        [Test]
        public void ThenIsFiltered_ShouldAddPipelineModule()
        {
            m_PipelineMock
                .Setup(x =>
                    x.AddModule<FilterPipelineModule, FilterPipelineModuleConfig>(
                        It.IsAny<FilterPipelineModuleConfig>()))
                .Verifiable();

            var eventPipelineConfigurator = m_EventPipelineConfigurator.ThenIsFiltered((x, y) => true);

            Assert.That(eventPipelineConfigurator, Is.EqualTo(m_EventPipelineConfigurator));
        }

        [Test]
        public void ThenIsFiltered_WithNullFilter_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_EventPipelineConfigurator.ThenIsFiltered(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TearDown]
        public void TearDown()
        {
            m_ServiceProviderMock.Verify();
            m_SourceModelsServiceMock.Verify();
            m_PipelineMock.Verify();
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
