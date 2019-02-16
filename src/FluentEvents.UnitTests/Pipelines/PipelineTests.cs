using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines
{
    [TestFixture]
    public class PipelineTests
    {
        private EventsScope m_EventsScope;
        private Mock<IServiceProvider> m_InternalServiceProviderMock;
        private Pipeline m_Pipeline;

        [SetUp]
        public void SetUp()
        {
            m_EventsScope = new EventsScope();
            m_InternalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_Pipeline = new Pipeline(m_InternalServiceProviderMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            m_InternalServiceProviderMock.Verify();
        }

        [Test]
        public void AddModuleConfig_ShouldAdd()
        {
            m_Pipeline.AddModule<PipelineModule1, object>(new object());
        }

        [Test]
        public void AddModuleConfig_WithNullModuleConfig_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_Pipeline.AddModule<PipelineModule1, object>(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task ProcessEventAsync_ShouldCreateAndDisposeNewServiceScope()
        {
            await m_Pipeline.ProcessEventAsync(new PipelineEvent(
                    typeof(object),
                    "f",
                    new object(),
                    new object()
                ),
                m_EventsScope
            );
        }

        [Test]
        public async Task ProcessEventAsync_ShouldBuildAndInvokeAllPipelineModules()
        {
            var pipelineModuleMocks = new List<Mock<IPipelineModule<object>>>();

            for (var i = 0; i < 4; i++)
            {
                var pipelineModuleMock = SetUpPipelineModule(i);

                pipelineModuleMocks.Add(pipelineModuleMock);
            }

            await m_Pipeline.ProcessEventAsync(new PipelineEvent(
                    typeof(object),
                    "f",
                    new object(),
                    new object()
                ),
                m_EventsScope
            );

            foreach (var pipelineModuleMock in pipelineModuleMocks)
                pipelineModuleMock.Verify();
        }

        private Mock<IPipelineModule<object>> SetUpPipelineModule(int index)
        {
            var pipelineModuleMock = new Mock<IPipelineModule<object>>(MockBehavior.Strict);
            var module = AddModule(index);

            pipelineModuleMock
                .Setup(x => x.InvokeAsync(It.IsAny<object>(), It.IsAny<PipelineContext>(), It.IsAny<NextModuleDelegate>()))
                .Callback<object, PipelineContext, NextModuleDelegate>(async (config, context, next) => await next(context))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var loggerType = typeof(ILogger<>).MakeGenericType(module.GetType());
            m_InternalServiceProviderMock
                .Setup(x => x.GetService(loggerType))
                .Returns(new LoggerFactory().CreateLogger(loggerType))
                .Verifiable();

            m_InternalServiceProviderMock
                .Setup(x => x.GetService(module.GetType()))
                .Returns(pipelineModuleMock.Object)
                .Verifiable();

            return pipelineModuleMock;
        }

        private object AddModule(int index)
        {
            IPipelineModule<object> module;

            switch (index)
            {
                case 0:
                    module = new PipelineModule1();
                    m_Pipeline.AddModule<PipelineModule1, object>(new object());
                    break;
                case 1:
                    module = new PipelineModule2();
                    m_Pipeline.AddModule<PipelineModule2, object>(new object());
                    break;
                case 2:
                    module = new PipelineModule3();
                    m_Pipeline.AddModule<PipelineModule3, object>(new object());
                    break;
                case 3:
                    module = new PipelineModule4();
                    m_Pipeline.AddModule<PipelineModule4, object>(new object());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return module;
        }
        
        private abstract class PipelineModuleBase : IPipelineModule<object>
        {
            public Task InvokeAsync(object config, PipelineContext pipelineContext, NextModuleDelegate invokeNextModule) 
                => Task.CompletedTask;
        }

        private class PipelineModule1 : PipelineModuleBase { }
        private class PipelineModule2 : PipelineModuleBase { }
        private class PipelineModule3 : PipelineModuleBase { }
        private class PipelineModule4 : PipelineModuleBase { }
    }
}
