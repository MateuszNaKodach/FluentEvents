using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Publication
{
    [TestFixture]
    public class GlobalPublishPipelineModuleTests : PipelineModuleTestBase
    {
        private GlobalPublishPipelineModuleConfig m_GlobalPublishPipelineModuleConfig;
        private GlobalPublishPipelineModule m_GlobalPublishPipelineModule;
        private Mock<IPublishingService> m_PublishingServiceMock;
        private Mock<IEventSender1> m_IEventSender1Mock;
        private Mock<IEventSender2> m_IEventSender2Mock;

        [SetUp]
        public void SetUp()
        {
            m_PublishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            m_IEventSender1Mock = new Mock<IEventSender1>(MockBehavior.Strict);
            m_IEventSender2Mock = new Mock<IEventSender2>(MockBehavior.Strict);
            m_GlobalPublishPipelineModule = new GlobalPublishPipelineModule(
                m_PublishingServiceMock.Object,
                new IEventSender[] { m_IEventSender1Mock.Object, m_IEventSender2Mock.Object }
            );
            m_GlobalPublishPipelineModuleConfig = new GlobalPublishPipelineModuleConfig();
        }

        [TearDown]
        public void TearDown()
        {
            m_PublishingServiceMock.Verify();
        }

        [Test]
        public async Task InvokeAsync_WhenSenderTypeIsNull_ShouldPublishToGlobalSubscriptionsImmediately()
        {
            var testSender = new TestSender();
            var testEventArgs = new TestEventArgs();

            var pipelineModuleContext = SetUpPipelineModuleContext(
                testSender,
                testEventArgs,
                m_GlobalPublishPipelineModuleConfig
            );

            PipelineContext nextModuleContext = null;

            Task InvokeNextModule(PipelineContext context)
            {
                nextModuleContext = context;
                return Task.CompletedTask;
            }

            m_PublishingServiceMock
                .Setup(x => x.PublishEventToGlobalSubscriptionsAsync(pipelineModuleContext.PipelineEvent))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await m_GlobalPublishPipelineModule.InvokeAsync(pipelineModuleContext, InvokeNextModule);

            Assert.That(nextModuleContext, Is.Not.Null);
            Assert.That(nextModuleContext, Is.EqualTo(pipelineModuleContext));
        }

        [Test]
        public async Task InvokeAsync_WhenSenderTypeIsNotNull_ShouldSendEvent()
        {
            var testSender = new TestSender();
            var testEventArgs = new TestEventArgs();

            m_GlobalPublishPipelineModuleConfig.SenderType = m_IEventSender1Mock.Object.GetType();

            var pipelineModuleContext = SetUpPipelineModuleContext(
                testSender,
                testEventArgs,
                m_GlobalPublishPipelineModuleConfig
            );

            PipelineContext nextModuleContext = null;

            Task InvokeNextModule(PipelineContext context)
            {
                nextModuleContext = context;
                return Task.CompletedTask;
            }

            m_IEventSender1Mock
                .Setup(x => x.SendAsync(pipelineModuleContext.PipelineEvent))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await m_GlobalPublishPipelineModule.InvokeAsync(pipelineModuleContext, InvokeNextModule);

            Assert.That(nextModuleContext, Is.Not.Null);
            Assert.That(nextModuleContext, Is.EqualTo(pipelineModuleContext));
        }

        [Test]
        public void InvokeAsync_WhenSenderIsNotFound_ShouldThrow()
        {
            var testSender = new TestSender();
            var testEventArgs = new TestEventArgs();

            m_GlobalPublishPipelineModuleConfig.SenderType = typeof(object);

            var pipelineModuleContext = SetUpPipelineModuleContext(
                testSender,
                testEventArgs,
                m_GlobalPublishPipelineModuleConfig
            );

            Task InvokeNextModule(PipelineContext pipelineContext) => Task.CompletedTask;

            Assert.That(async () =>
            {
                await m_GlobalPublishPipelineModule.InvokeAsync(pipelineModuleContext, InvokeNextModule);
            }, Throws.TypeOf<EventSenderNotFoundException>());
        }


        public interface IEventSender1 : IEventSender { }
        public interface IEventSender2 : IEventSender { }

        private class TestSender { }
        private class TestEventArgs { }
    }
}
