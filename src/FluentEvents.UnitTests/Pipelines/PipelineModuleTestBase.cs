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

        protected Mock<IEventsScope> EventsScopeMock { get; private set; }

        [SetUp]
        public void BaseSetUp()
        {
            _internalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            EventsScopeMock = new Mock<IEventsScope>(MockBehavior.Strict);
        }

        [TearDown]
        public void BaseTearDown()
        {
            _internalServiceProviderMock.Verify();
            EventsScopeMock.Verify();
        }

        protected PipelineContext CreatePipelineContext(object e)
        {
            return new PipelineContext(
                new PipelineEvent(e),
                EventsScopeMock.Object,
                _internalServiceProviderMock.Object
            );
        }
    }
}
