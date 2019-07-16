using System;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Routing;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Routing
{
    [TestFixture]
    public class AttachingServiceTests
    {
        private Mock<ISourceModelsService> _sourceModelsServiceMock;
        private Mock<IForwardingService> _forwardingServiceMock;
        private Mock<IAttachingInterceptor> _attachingInterceptorMock1;
        private Mock<IAttachingInterceptor> _attachingInterceptorMock2;
        private Mock<IEventsScope> _eventsScopeMock;

        private IAttachingService _attachingService;

        [SetUp]
        public void SetUp()
        {
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _forwardingServiceMock = new Mock<IForwardingService>(MockBehavior.Strict);
            _attachingInterceptorMock1 = new Mock<IAttachingInterceptor>(MockBehavior.Strict);
            _attachingInterceptorMock2 = new Mock<IAttachingInterceptor>(MockBehavior.Strict);
            _eventsScopeMock = new Mock<IEventsScope>(MockBehavior.Strict);

            _attachingService = new AttachingService(
                _sourceModelsServiceMock.Object,
                _forwardingServiceMock.Object,
                new[]
                {
                    _attachingInterceptorMock1.Object,
                    _attachingInterceptorMock2.Object
                }
            );
        }

        [TearDown]
        public void TearDown()
        {
            _sourceModelsServiceMock.Verify();
            _forwardingServiceMock.Verify();
            _attachingInterceptorMock1.Verify();
            _attachingInterceptorMock2.Verify();
        }

        [Test]
        [Sequential]
        public void Attach_WithNullParameters_ShouldThrow(
            [Values(true, false, true)] bool isSourceNull,
            [Values(false, true, true)] bool isEventsScopeNull
        )
        {
            var source = isSourceNull ? null : new object();
            var eventsScope = isEventsScopeNull ? null : _eventsScopeMock.Object;
            
            Assert.That(() =>
            {
                _attachingService.Attach(source, eventsScope);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Attach_ShouldIterateBaseTypesAndAttach()
        {
            var source = new Source3();

            SetUpInterceptors(source);
            
            SetUpSourceModelsServiceAndForwardingService(typeof(object), source);
            SetUpSourceModelsServiceAndForwardingService(typeof(Source3), source);
            SetUpSourceModelsServiceAndForwardingService(typeof(Source2), source);
            SetUpSourceModelsServiceAndForwardingService(typeof(Source1), source);
            SetUpSourceModelsServiceAndForwardingService(typeof(ISource), source);
            
            _attachingService.Attach(source, _eventsScopeMock.Object);
        }

        private void SetUpSourceModelsServiceAndForwardingService(Type type, Source3 source)
        {
            var sourceModel = new SourceModel(type);

            _sourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(type))
                .Returns(sourceModel)
                .Verifiable();

            _forwardingServiceMock
                .Setup(x => x.ForwardEventsToRouting(sourceModel, source, _eventsScopeMock.Object))
                .Verifiable();
        }

        private void SetUpInterceptors(object source)
        {
            _attachingInterceptorMock1
                .Setup(x => x.OnAttaching(_attachingService, source, _eventsScopeMock.Object))
                .Verifiable();

            _attachingInterceptorMock2
                .Setup(x => x.OnAttaching(_attachingService, source, _eventsScopeMock.Object))
                .Verifiable();
        }

        private class Source1 { }

        private class Source2 : Source1, ISource { }

        private class Source3 : Source2 { }

        private interface ISource
        {
        }
    }
}
