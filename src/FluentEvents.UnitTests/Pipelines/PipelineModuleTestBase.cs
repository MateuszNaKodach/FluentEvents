using System;
using FluentEvents.Pipelines;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines
{
    public class PipelineModuleTestBase
    {
        protected Mock<IServiceProvider> InternalServiceProviderMock { get; private set; }
        protected EventsScope EventsScope { get; private set; }

        [SetUp]
        public void BaseSetUp()
        {
            InternalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            EventsScope = new EventsScope();
        }

        [TearDown]
        public void BaseTearDown()
        {
            InternalServiceProviderMock.Verify();
        }

        protected PipelineContext CreatePipelineContext(object testSender, object testEventArgs)
        {
            return new PipelineContext(
                new PipelineEvent(typeof(object), "f", testSender, testEventArgs),
                EventsScope,
                InternalServiceProviderMock.Object
            );
        }
    }
}
