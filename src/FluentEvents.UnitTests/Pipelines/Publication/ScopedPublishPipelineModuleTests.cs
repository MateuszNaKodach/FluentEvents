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
        private Mock<IPublishingService> m_PublishingServiceMock;
        private ScopedPublishPipelineModule m_ScopedPublishPipelineModule;
        private ScopedPublishPipelineModuleConfig m_ScopedPublishPipelineModuleConfig;

        [SetUp]
        public void SetUp()
        {
            m_PublishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            m_ScopedPublishPipelineModuleConfig = new ScopedPublishPipelineModuleConfig();
            m_ScopedPublishPipelineModule = new ScopedPublishPipelineModule(m_PublishingServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            m_PublishingServiceMock.Verify();
        }

        [Test]
        public async Task InvokeAsync_ShouldPublishEventInScope()
        {
            var pipelineModuleContext = SetUpPipelineModuleContext(
                new object(),
                new object(),
                m_ScopedPublishPipelineModuleConfig
            );

            PipelineContext nextModuleContext = null;

            Task InvokeNextModule(PipelineContext context)
            {
                nextModuleContext = context;
                return Task.CompletedTask;
            }

            m_PublishingServiceMock
                .Setup(x => x.PublishEventToScopedSubscriptionsAsync(pipelineModuleContext.PipelineEvent, EventsScope))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await m_ScopedPublishPipelineModule.InvokeAsync(pipelineModuleContext, InvokeNextModule);

            Assert.That(nextModuleContext, Is.Not.Null);
            Assert.That(nextModuleContext, Is.EqualTo(pipelineModuleContext));
        }
    }
}
