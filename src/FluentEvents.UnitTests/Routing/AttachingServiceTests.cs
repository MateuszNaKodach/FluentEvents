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
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;
        private Mock<IForwardingService> m_ForwardingServiceMock;
        private AttachingService m_AttachingService;
        private EventsScope m_EventsScope;
        private SourceModel m_Source2SourceModel;

        [SetUp]
        public void SetUp()
        {
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            m_ForwardingServiceMock = new Mock<IForwardingService>(MockBehavior.Strict);
            m_AttachingService = new AttachingService(
                m_SourceModelsServiceMock.Object,
                m_ForwardingServiceMock.Object
            );
            m_EventsScope = new EventsScope();
            m_Source2SourceModel = new SourceModel(typeof(Source2));
        }

        [TearDown]
        public void TearDown()
        {
            m_SourceModelsServiceMock.Verify();
            m_ForwardingServiceMock.Verify();
        }

        [Test]
        [Sequential]
        public void Attach_WithNullParameters_ShouldThrow(
            [Values(true, false, true)] bool isSourceNull,
            [Values(false, true, true)] bool isEventsScopeNull
        )
        {
            var source = isSourceNull ? null : new object();
            var eventsScope = isEventsScopeNull ? null : m_EventsScope;
            
            Assert.That(() =>
            {
                m_AttachingService.Attach(source, eventsScope);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Attach_WithExistingSourceModel_ShouldIterateBaseTypesAndAttach()
        {
            var source = new Source3();

            m_SourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(Source3)))
                .Returns<SourceModel>(null)
                .Verifiable();

            m_SourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(Source2)))
                .Returns(m_Source2SourceModel)
                .Verifiable();

            m_ForwardingServiceMock
                .Setup(x => x.ForwardEventsToRouting(m_Source2SourceModel, source, m_EventsScope))
                .Verifiable();

            m_AttachingService.Attach(source, m_EventsScope);
        }

        [Test]
        public void Attach_WithNoExistingSourceModel_ShouldIterateBaseTypesAndNotAttach()
        {
            var source = new Source3();

            m_SourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(Source3)))
                .Returns<SourceModel>(null)
                .Verifiable();

            m_SourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(Source2)))
                .Returns<SourceModel>(null)
                .Verifiable();

            m_SourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(Source1)))
                .Returns<SourceModel>(null)
                .Verifiable();

            m_SourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(object)))
                .Returns<SourceModel>(null)
                .Verifiable();

            m_AttachingService.Attach(source, m_EventsScope);
        }

        public class Source1 { }

        public class Source2 : Source1 { }

        public class Source3 : Source2 { }
    }
}
