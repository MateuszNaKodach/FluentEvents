using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Projections;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Projections
{
    [TestFixture]
    public class ProjectionPipelineModuleTests : PipelineModuleTestBase
    {
        private ProjectionPipelineModule m_ProjectionPipelineModule;
        private ProjectionPipelineModuleConfig m_ProjectionPipelineModuleConfig;
        private Mock<IEventsSenderProjection> m_EventSenderProjectionMock;
        private Mock<IEventArgsProjection> m_EventArgsProjectionMock;

        [SetUp]
        public void SetUp()
        {
            m_EventSenderProjectionMock = new Mock<IEventsSenderProjection>(MockBehavior.Strict);
            m_EventArgsProjectionMock = new Mock<IEventArgsProjection>(MockBehavior.Strict);
            m_ProjectionPipelineModule = new ProjectionPipelineModule();
            m_ProjectionPipelineModuleConfig = new ProjectionPipelineModuleConfig(
                m_EventSenderProjectionMock.Object,
                m_EventArgsProjectionMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_EventSenderProjectionMock.Verify();
            m_EventArgsProjectionMock.Verify();
        }

        [Test]
        public async Task InvokeAsync_ShouldProjectInvokeNextModule()
        {
            var testSender = new TestSender();
            var testEventArgs = new TestEventArgs();
            var projectedTestSender = new ProjectedTestSender();
            var projectedTestEventArgs = new ProjectedTestEventArgs();

            var pipelineContext = CreatePipelineContext(
                testSender, 
                testEventArgs
            );

            m_EventSenderProjectionMock
                .Setup(x => x.Convert(testSender))
                .Returns(projectedTestSender)
                .Verifiable();

            m_EventArgsProjectionMock
                .Setup(x => x.Convert(testEventArgs))
                .Returns(projectedTestEventArgs)
                .Verifiable();

            PipelineContext nextModuleContext = null;

            Task InvokeNextModule(PipelineContext context)
            {
                nextModuleContext = context;
                return Task.CompletedTask;
            }

            await m_ProjectionPipelineModule.InvokeAsync(m_ProjectionPipelineModuleConfig, pipelineContext, InvokeNextModule);

            Assert.That(nextModuleContext, Is.Not.Null);
            Assert.That(nextModuleContext, Is.EqualTo(pipelineContext));
            Assert.That(
                nextModuleContext.PipelineEvent,
                Has.Property(nameof(PipelineEvent.OriginalSender)).EqualTo(projectedTestSender)
            );
            Assert.That(
                nextModuleContext.PipelineEvent,
                Has.Property(nameof(PipelineEvent.OriginalEventArgs)).EqualTo(projectedTestEventArgs)
            );
        }

        private class TestSender
        {
        }

        private class TestEventArgs
        {
        }

        private class ProjectedTestSender
        {
        }

        private class ProjectedTestEventArgs
        {
        }
    }
}
