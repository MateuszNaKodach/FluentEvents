using System;
using FluentEvents.Pipelines;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines
{
    public class PipelineModuleTestBase
    {
        private Mock<IServiceProvider> _internalServiceProviderMock;

        protected EventsScope EventsScope { get; private set; }

        [SetUp]
        public void BaseSetUp()
        {
            _internalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);

            EventsScope = new EventsScope();
        }

        [TearDown]
        public void BaseTearDown()
        {
            _internalServiceProviderMock.Verify();
        }

        protected PipelineContext CreatePipelineContext(object e)
        {
            return new PipelineContext(
                new PipelineEvent(e),
                EventsScope,
                _internalServiceProviderMock.Object
            );
        }
    }
}
