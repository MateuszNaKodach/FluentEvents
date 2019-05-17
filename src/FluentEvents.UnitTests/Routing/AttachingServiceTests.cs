using System;
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
        private IAttachingService _attachingService;
        private EventsScope _eventsScope;
        private SourceModel _source2SourceModel;

        [SetUp]
        public void SetUp()
        {
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _forwardingServiceMock = new Mock<IForwardingService>(MockBehavior.Strict);
            _attachingInterceptorMock1 = new Mock<IAttachingInterceptor>(MockBehavior.Strict);
            _attachingInterceptorMock2 = new Mock<IAttachingInterceptor>(MockBehavior.Strict);
            _attachingService = new AttachingService(
                _sourceModelsServiceMock.Object,
                _forwardingServiceMock.Object,
                new[]
                {
                    _attachingInterceptorMock1.Object,
                    _attachingInterceptorMock2.Object
                }
            );
            _eventsScope = new EventsScope();
            _source2SourceModel = new SourceModel(typeof(Source2));
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
            var eventsScope = isEventsScopeNull ? null : _eventsScope;
            
            Assert.That(() =>
            {
                _attachingService.Attach(source, eventsScope);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Attach_WithExistingSourceModel_ShouldIterateBaseTypesAndAttach()
        {
            var source = new Source3();

            SetUpInterceptors(source);

            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(Source3)))
                .Returns<SourceModel>(null)
                .Verifiable();

            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(Source2)))
                .Returns(_source2SourceModel)
                .Verifiable();

            _forwardingServiceMock
                .Setup(x => x.ForwardEventsToRouting(_source2SourceModel, source, _eventsScope))
                .Verifiable();

            _attachingService.Attach(source, _eventsScope);
        }

        [Test]
        public void Attach_WithNoExistingSourceModel_ShouldIterateBaseTypesAndNotAttach()
        {
            var source = new Source3();

            SetUpInterceptors(source);

            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(Source3)))
                .Returns<SourceModel>(null)
                .Verifiable();

            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(Source2)))
                .Returns<SourceModel>(null)
                .Verifiable();

            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(Source1)))
                .Returns<SourceModel>(null)
                .Verifiable();

            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(object)))
                .Returns<SourceModel>(null)
                .Verifiable();

            _attachingService.Attach(source, _eventsScope);
        }

        private void SetUpInterceptors(object source)
        {
            _attachingInterceptorMock1
                .Setup(x => x.OnAttaching(_attachingService, source, _eventsScope))
                .Verifiable();

            _attachingInterceptorMock2
                .Setup(x => x.OnAttaching(_attachingService, source, _eventsScope))
                .Verifiable();
        }

        public class Source1 { }

        public class Source2 : Source1 { }

        public class Source3 : Source2 { }
    }
}
