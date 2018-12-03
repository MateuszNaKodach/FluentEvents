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
        private EventsContextImpl m_EventsContext;
        private EventsScope m_EventsScope;
        private Mock<IServiceProvider> m_InternalServiceProviderMock;
        private Mock<IServiceProvider> m_ScopedServiceProviderMock;
        private Mock<IServiceScopeFactory> m_ServiceScopeFactoryMock;
        private Mock<IServiceScope> m_ServiceScopeMock;
        private Mock<IPipelineModuleConfig> m_PipelineModuleConfigMock;
        private Pipeline m_Pipeline;

        [SetUp]
        public void SetUp()
        {
            m_EventsContext = new EventsContextImpl();
            m_EventsScope = new EventsScope();
            m_InternalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_ScopedServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_ServiceScopeFactoryMock = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
            m_ServiceScopeMock = new Mock<IServiceScope>(MockBehavior.Strict);
            m_PipelineModuleConfigMock = new Mock<IPipelineModuleConfig>(MockBehavior.Strict);
            m_Pipeline = new Pipeline(null, m_EventsContext, m_InternalServiceProviderMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            m_InternalServiceProviderMock.Verify();
            m_ServiceScopeFactoryMock.Verify();
            m_ServiceScopeMock.Verify();
            m_PipelineModuleConfigMock.Verify();
        }

        [Test]
        public void AddModuleConfig_ShouldAdd()
        {
            m_Pipeline.AddModuleConfig(m_PipelineModuleConfigMock.Object);
        }

        [Test]
        public void AddModuleConfig_WithNullModuleConfig_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_Pipeline.AddModuleConfig(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task ProcessEventAsync_ShouldCreateAndDisposeNewServiceScope()
        {
            SetUpCreateScope();
            await m_Pipeline.ProcessEventAsync(new PipelineEvent("f", new object(), new object()), m_EventsScope);
        }

        [Test]
        public async Task ProcessEventAsync_ShouldBuildAndInvokeAllPipelineModules()
        {
            var pipelineModuleConfigMocks = new List<Mock<IPipelineModuleConfig>>();
            var pipelineModuleMocks = new List<Mock<IPipelineModule>>();

            SetUpCreateScope();
            for (var i = 0; i < 4; i++)
            {
                var (pipelineModuleConfigMock, pipelineModuleMock) = SetUpPipelineModule(i);

                pipelineModuleConfigMocks.Add(pipelineModuleConfigMock);
                pipelineModuleMocks.Add(pipelineModuleMock);
            }

            await m_Pipeline.ProcessEventAsync(new PipelineEvent("f", new object(), new object()), m_EventsScope);

            foreach (var pipelineModuleConfigMock in pipelineModuleConfigMocks)
                pipelineModuleConfigMock.Verify();
            
            foreach (var pipelineModuleMock in pipelineModuleMocks)
                pipelineModuleMock.Verify();
        }

        private (Mock<IPipelineModuleConfig>, Mock<IPipelineModule>) SetUpPipelineModule(int index)
        {
            var pipelineModuleConfigMock = new Mock<IPipelineModuleConfig>(MockBehavior.Strict);
            var pipelineModuleMock = new Mock<IPipelineModule>(MockBehavior.Strict);
            var module = GetModule(index);

            pipelineModuleMock
                .Setup(x => x.InvokeAsync(It.IsAny<PipelineModuleContext>(), It.IsAny<NextModuleDelegate>()))
                .Callback<PipelineModuleContext, NextModuleDelegate>(async (context, next) => await next(context))
                .Returns(Task.CompletedTask)
                .Verifiable();

            pipelineModuleConfigMock
                .Setup(x => x.ModuleType)
                .Returns(module.GetType())
                .Verifiable();

            var loggerType = typeof(ILogger<>).MakeGenericType(pipelineModuleConfigMock.Object.ModuleType);
            m_ScopedServiceProviderMock
                .Setup(x => x.GetService(loggerType))
                .Returns(new LoggerFactory().CreateLogger(loggerType))
                .Verifiable();

            m_ScopedServiceProviderMock
                .Setup(x => x.GetService(pipelineModuleConfigMock.Object.ModuleType))
                .Returns(pipelineModuleMock.Object)
                .Verifiable();

            m_Pipeline.AddModuleConfig(pipelineModuleConfigMock.Object);

            return (pipelineModuleConfigMock, pipelineModuleMock);
        }

        private object GetModule(int index)
        {
            switch (index)
            {
                case 0:
                    return new PipelineModule1();
                case 1:
                    return new PipelineModule2();
                case 2:
                    return new PipelineModule3();
                case 3:
                    return new PipelineModule4();
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
