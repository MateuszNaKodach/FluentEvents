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
        private Mock<IServiceProvider> m_ScopedServiceProviderMock;
        private Mock<IServiceScopeFactory> m_ServiceScopeFactoryMock;
        private Mock<IServiceScope> m_ServiceScopeMock;
        private Pipeline m_Pipeline;

        [SetUp]
        public void SetUp()
        {
            m_EventsScope = new EventsScope();
            m_InternalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_ScopedServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_ServiceScopeFactoryMock = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
            m_ServiceScopeMock = new Mock<IServiceScope>(MockBehavior.Strict);
            m_Pipeline = new Pipeline(null, m_InternalServiceProviderMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            m_InternalServiceProviderMock.Verify();
            m_ScopedServiceProviderMock.Verify();
            m_ServiceScopeFactoryMock.Verify();
            m_ServiceScopeMock.Verify();
        }

        [Test]
        public void AddModuleConfig_ShouldAdd()
        {
            m_Pipeline.AddModule<PipelineModule1>(new object());
        }

        [Test]
        public void AddModuleConfig_WithNullModuleConfig_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_Pipeline.AddModule<PipelineModule1>(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task ProcessEventAsync_ShouldCreateAndDisposeNewServiceScope()
        {
            SetUpCreateScope();

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
            var pipelineModuleMocks = new List<Mock<IPipelineModule>>();

            SetUpCreateScope();
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

        private Mock<IPipelineModule> SetUpPipelineModule(int index)
        {
            var pipelineModuleMock = new Mock<IPipelineModule>(MockBehavior.Strict);
            var module = AddModule(index);

            pipelineModuleMock
                .Setup(x => x.InvokeAsync(It.IsAny<PipelineModuleContext>(), It.IsAny<NextModuleDelegate>()))
                .Callback<PipelineModuleContext, NextModuleDelegate>(async (context, next) => await next(context))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var loggerType = typeof(ILogger<>).MakeGenericType(module.GetType());
            m_ScopedServiceProviderMock
                .Setup(x => x.GetService(loggerType))
                .Returns(new LoggerFactory().CreateLogger(loggerType))
                .Verifiable();

            m_ScopedServiceProviderMock
                .Setup(x => x.GetService(module.GetType()))
                .Returns(pipelineModuleMock.Object)
                .Verifiable();

            return pipelineModuleMock;
        }

        private object AddModule(int index)
        {
            IPipelineModule module;

            switch (index)
            {
                case 0:
                    module = new PipelineModule1();
                    m_Pipeline.AddModule<PipelineModule1>(new object());
                    break;
                case 1:
                    module = new PipelineModule2();
                    m_Pipeline.AddModule<PipelineModule2>(new object());
                    break;
                case 2:
                    module = new PipelineModule3();
                    m_Pipeline.AddModule<PipelineModule3>(new object());
                    break;
                case 3:
                    module = new PipelineModule4();
                    m_Pipeline.AddModule<PipelineModule4>(new object());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return module;
        }

        private void SetUpCreateScope()
        {
            m_InternalServiceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(m_ServiceScopeFactoryMock.Object)
                .Verifiable();

            m_ServiceScopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(m_ServiceScopeMock.Object)
                .Verifiable();

            m_ServiceScopeMock
                .Setup(x => x.ServiceProvider)
                .Returns(m_ScopedServiceProviderMock.Object)
                .Verifiable();

            m_ServiceScopeMock
                .Setup(x => x.Dispose())
                .Verifiable();
        }

        private abstract class PipelineModuleBase : IPipelineModule
        {
            public Task InvokeAsync(PipelineModuleContext pipelineModuleContext, NextModuleDelegate invokeNextModule) => Task.CompletedTask;
        }

        private class PipelineModule1 : PipelineModuleBase { }
        private class PipelineModule2 : PipelineModuleBase { }
        private class PipelineModule3 : PipelineModuleBase { }
        private class PipelineModule4 : PipelineModuleBase { }
    }
}
