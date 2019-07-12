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
        private Mock<IEventProjection> _eventProjectionMock;
        private ProjectionPipelineModuleConfig _projectionPipelineModuleConfig;

        private ProjectionPipelineModule _projectionPipelineModule;

        [SetUp]
        public void SetUp()
        {
            _eventProjectionMock = new Mock<IEventProjection>(MockBehavior.Strict);
            _projectionPipelineModuleConfig = new ProjectionPipelineModuleConfig(
                _eventProjectionMock.Object
            );

            _projectionPipelineModule = new ProjectionPipelineModule();
        }

        [TearDown]
        public void TearDown()
        {
            _eventProjectionMock.Verify();
        }

        [Test]
        public async Task InvokeAsync_ShouldProjectInvokeNextModule()
        {
            var testEventArgs = new TestEvent();
            var projectedTestEvent = new ProjectedTestEvent();

            var pipelineContext = CreatePipelineContext(testEventArgs);
            
            _eventProjectionMock
                .Setup(x => x.Convert(testEventArgs))
                .Returns(projectedTestEvent)
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
                Has.Property(nameof(PipelineEvent.Event)).EqualTo(projectedTestEvent)
            );
        }

        private class TestEvent
        {
        }

        private class ProjectedTestEvent
        {
        }
    }
}
