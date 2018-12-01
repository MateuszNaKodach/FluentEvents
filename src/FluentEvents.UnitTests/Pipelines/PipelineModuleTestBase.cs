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

        protected PipelineModuleContext SetUpPipelineModuleContext(object testSender, object testEventArgs, IPipelineModuleConfig projectionPipelineModuleConfig)
        {
            return new PipelineModuleContext(
                projectionPipelineModuleConfig,
                new PipelineContext(
                    new PipelineEvent("f", testSender, testEventArgs),
                    EventsScope,
                    InternalServiceProviderMock.Object
                )
            );
        }
    }
}
