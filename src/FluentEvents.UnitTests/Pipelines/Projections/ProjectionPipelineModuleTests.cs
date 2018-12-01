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
        private Mock<IEventsSenderProjection> m_EventSenderProjection;
        private Mock<IEventArgsProjection> m_EventArgsProjection;

        [SetUp]
        public void SetUp()
        {
            m_EventSenderProjection = new Mock<IEventsSenderProjection>(MockBehavior.Strict);
            m_EventArgsProjection = new Mock<IEventArgsProjection>(MockBehavior.Strict);
            m_ProjectionPipelineModule = new ProjectionPipelineModule();
            m_ProjectionPipelineModuleConfig = new ProjectionPipelineModuleConfig(
                m_EventSenderProjection.Object,
                m_EventArgsProjection.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_EventSenderProjection.Verify();
            m_EventArgsProjection.Verify();
        }

        [Test]
        public async Task InvokeAsync_ShouldProjectInvokeNextModule()
        {
            var testSender = new TestSender();
            var testEventArgs = new TestEventArgs();
            var projectedTestSender = new ProjectedTestSender();
            var projectedTestEventArgs = new ProjectedTestEventArgs();

            var pipelineModuleContext = SetUpPipelineModuleContext(
                testSender, 
                testEventArgs, 
                m_ProjectionPipelineModuleConfig
            );

            m_EventSenderProjection
                .Setup(x => x.Convert(testSender))
                .Returns(projectedTestSender)
                .Verifiable();

            m_EventArgsProjection
                .Setup(x => x.Convert(testEventArgs))
                .Returns(projectedTestEventArgs)
                .Verifiable();

            PipelineContext nextModuleContext = null;

            Task InvokeNextModule(PipelineContext context)
            {
                nextModuleContext = context;
                return Task.CompletedTask;
            }

            await m_ProjectionPipelineModule.InvokeAsync(pipelineModuleContext, InvokeNextModule);

            Assert.That(nextModuleContext, Is.Not.Null);
            Assert.That(nextModuleContext, Is.EqualTo(pipelineModuleContext));
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
