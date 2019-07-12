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
        private GlobalPublishPipelineModuleConfig _globalPublishPipelineModuleConfig;
        private GlobalPublishPipelineModule _globalPublishPipelineModule;
        private Mock<IPublishingService> _publishingServiceMock;
        private Mock<IEventSender1> _eventSender1Mock;
        private Mock<IEventSender2> _eventSender2Mock;

        [SetUp]
        public void SetUp()
        {
            _publishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            _eventSender1Mock = new Mock<IEventSender1>(MockBehavior.Strict);
            _eventSender2Mock = new Mock<IEventSender2>(MockBehavior.Strict);
            _globalPublishPipelineModule = new GlobalPublishPipelineModule(
                _publishingServiceMock.Object,
                new IEventSender[] { _eventSender1Mock.Object, _eventSender2Mock.Object }
            );
            _globalPublishPipelineModuleConfig = new GlobalPublishPipelineModuleConfig();
        }

        [TearDown]
        public void TearDown()
        {
            _publishingServiceMock.Verify();
            _eventSender1Mock.Verify();
            _eventSender2Mock.Verify();
        }

        [Test]
        public async Task InvokeAsync_WhenSenderTypeIsNull_ShouldPublishToGlobalSubscriptionsImmediately()
        {
            var testSender = new TestSender();
            var testEventArgs = new TestEventArgs();

            var pipelineContext = CreatePipelineContext(testEventArgs
            );

            PipelineContext nextModuleContext = null;

            Task InvokeNextModule(PipelineContext context)
            {
                nextModuleContext = context;
                return Task.CompletedTask;
            }

            _publishingServiceMock
                .Setup(x => x.PublishEventToGlobalSubscriptionsAsync(pipelineContext.PipelineEvent))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _globalPublishPipelineModule.InvokeAsync(_globalPublishPipelineModuleConfig, pipelineContext, InvokeNextModule);

            Assert.That(nextModuleContext, Is.Not.Null);
            Assert.That(nextModuleContext, Is.EqualTo(pipelineContext));
        }

        [Test]
        public async Task InvokeAsync_WhenSenderTypeIsNotNull_ShouldSendEvent()
        {
            var testSender = new TestSender();
            var testEventArgs = new TestEventArgs();

            _globalPublishPipelineModuleConfig.SenderType = _eventSender1Mock.Object.GetType();

            var pipelineContext = CreatePipelineContext(testEventArgs
            );

            PipelineContext nextModuleContext = null;

            Task InvokeNextModule(PipelineContext context)
            {
                nextModuleContext = context;
                return Task.CompletedTask;
            }

            _eventSender1Mock
                .Setup(x => x.SendAsync(pipelineContext.PipelineEvent))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _globalPublishPipelineModule.InvokeAsync(_globalPublishPipelineModuleConfig, pipelineContext, InvokeNextModule);

            Assert.That(nextModuleContext, Is.Not.Null);
            Assert.That(nextModuleContext, Is.EqualTo(pipelineContext));
        }

        [Test]
        public void InvokeAsync_WhenSenderIsNotFound_ShouldThrow()
        {
            var testSender = new TestSender();
            var testEventArgs = new TestEventArgs();

            _globalPublishPipelineModuleConfig.SenderType = typeof(object);

            var pipelineContext = CreatePipelineContext(testEventArgs
            );

            Task InvokeNextModule(PipelineContext context) => Task.CompletedTask;

            Assert.That(async () =>
            {
                await _globalPublishPipelineModule.InvokeAsync(_globalPublishPipelineModuleConfig, pipelineContext, InvokeNextModule);
            }, Throws.TypeOf<EventSenderNotFoundException>());
        }


        public interface IEventSender1 : IEventSender { }
        public interface IEventSender2 : IEventSender { }

        private class TestSender { }
        private class TestEventArgs { }
    }
}
