using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Publication
{
    [TestFixture]
    public class ScopedPublishPipelineModuleTests : PipelineModuleTestBase
    {
        private Mock<IPublishingService> _publishingServiceMock;
        private ScopedPublishPipelineModule _scopedPublishPipelineModule;
        private ScopedPublishPipelineModuleConfig _scopedPublishPipelineModuleConfig;

        [SetUp]
        public void SetUp()
        {
            _publishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            _scopedPublishPipelineModuleConfig = new ScopedPublishPipelineModuleConfig();
            _scopedPublishPipelineModule = new ScopedPublishPipelineModule(_publishingServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _publishingServiceMock.Verify();
        }

        [Test]
        public async Task InvokeAsync_ShouldPublishEventInScope()
        {
            var pipelineContext = CreatePipelineContext(
                new object(),
                new object()
            );

            PipelineContext nextModuleContext = null;

            Task InvokeNextModule(PipelineContext context)
            {
                nextModuleContext = context;
                return Task.CompletedTask;
            }

            _publishingServiceMock
                .Setup(x => x.PublishEventToScopedSubscriptionsAsync(pipelineContext.PipelineEvent, EventsScope))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _scopedPublishPipelineModule.InvokeAsync(_scopedPublishPipelineModuleConfig, pipelineContext, InvokeNextModule);

            Assert.That(nextModuleContext, Is.Not.Null);
            Assert.That(nextModuleContext, Is.EqualTo(pipelineContext));
        }
    }
}
