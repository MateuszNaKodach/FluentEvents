using System;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines
{
    public class PipelineModuleTestBase
    {
        protected Mock<IEventsScope> EventsScopeMock { get; private set; }

        [SetUp]
        public void BaseSetUp()
        {
            EventsScopeMock = new Mock<IEventsScope>(MockBehavior.Strict);
        }

        [TearDown]
        public void BaseTearDown()
        {
            EventsScopeMock.Verify();
        }

        protected PipelineContext CreatePipelineContext(object e)
        {
            return new PipelineContext(
                new PipelineEvent(e),
                EventsScopeMock.Object
            );
        }
    }
}
