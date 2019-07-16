using System;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines
{
    public class PipelineModuleTestBase
    {
        private Mock<IServiceProvider> _internalServiceProviderMock;

        protected Mock<IEventsScope> EventsScope { get; private set; }

        [SetUp]
        public void BaseSetUp()
        {
            _internalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            EventsScope = new Mock<IEventsScope>(MockBehavior.Strict);
        }

        [TearDown]
        public void BaseTearDown()
        {
            _internalServiceProviderMock.Verify();
            EventsScope.Verify();
        }

        protected PipelineContext CreatePipelineContext(object e)
        {
            return new PipelineContext(
                new PipelineEvent(e),
                EventsScope.Object,
                _internalServiceProviderMock.Object
            );
        }
    }
}
