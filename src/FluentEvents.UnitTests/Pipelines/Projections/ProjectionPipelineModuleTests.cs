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
        private ProjectionPipelineModule _projectionPipelineModule;
        private ProjectionPipelineModuleConfig _projectionPipelineModuleConfig;
        private Mock<IEventsSenderProjection> _eventSenderProjectionMock;
        private Mock<IEventArgsProjection> _eventArgsProjectionMock;

        [SetUp]
        public void SetUp()
        {
            _eventSenderProjectionMock = new Mock<IEventsSenderProjection>(MockBehavior.Strict);
            _eventArgsProjectionMock = new Mock<IEventArgsProjection>(MockBehavior.Strict);
            _projectionPipelineModule = new ProjectionPipelineModule();
            _projectionPipelineModuleConfig = new ProjectionPipelineModuleConfig(
                _eventSenderProjectionMock.Object,
                _eventArgsProjectionMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _eventSenderProjectionMock.Verify();
            _eventArgsProjectionMock.Verify();
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

            _eventSenderProjectionMock
                .Setup(x => x.Convert(testSender))
                .Returns(projectedTestSender)
                .Verifiable();

            _eventArgsProjectionMock
                .Setup(x => x.Convert(testEventArgs))
                .Returns(projectedTestEventArgs)
                .Verifiable();

            PipelineContext nextModuleContext = null;

            Task InvokeNextModule(PipelineContext context)
            {
                nextModuleContext = context;
                return Task.CompletedTask;
            }

            await _projectionPipelineModule.InvokeAsync(_projectionPipelineModuleConfig, pipelineContext, InvokeNextModule);

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
