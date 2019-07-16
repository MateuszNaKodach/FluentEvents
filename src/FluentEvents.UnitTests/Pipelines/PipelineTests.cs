using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines
{
    [TestFixture]
    public class PipelineTests
    {
        private Mock<IEventsScope> _eventsScopeMock;
        private Mock<IServiceProvider> _internalServiceProviderMock;
        private Pipeline _pipeline;

        [SetUp]
        public void SetUp()
        {
            _eventsScopeMock = new Mock<IEventsScope>(MockBehavior.Strict);
            _internalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _pipeline = new Pipeline(_internalServiceProviderMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _eventsScopeMock.Verify();
            _internalServiceProviderMock.Verify();
        }

        [Test]
        public void AddModuleConfig_WithModuleFoundByServiceProvider_ShouldAdd()
        {
            _internalServiceProviderMock
                .Setup(x => x.GetService(typeof(PipelineModule1)))
                .Returns(new PipelineModule1())
                .Verifiable();

            _pipeline.AddModule<PipelineModule1, object>(new object());
        }

        [Test]
        public void AddModuleConfig_WithModuleNotFoundByServiceProvider_ShouldThrow()
        {
            _internalServiceProviderMock
                .Setup(x => x.GetService(typeof(PipelineModule1)))
                .Returns(null)
                .Verifiable();

            Assert.That(() =>
            {
                _pipeline.AddModule<PipelineModule1, object>(new object());
            }, Throws.TypeOf<PipelineModuleNotFoundException>());
        }

        [Test]
        public void AddModuleConfig_WithNullModuleConfig_ShouldThrow()
        {
            Assert.That(() =>
            {
                _pipeline.AddModule<PipelineModule1, object>(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task ProcessEventAsync_ShouldCreateAndDisposeNewServiceScope()
        {
            await _pipeline.ProcessEventAsync(new PipelineEvent(typeof(object)), _eventsScopeMock.Object);
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

            await _pipeline.ProcessEventAsync(new PipelineEvent(new object()), _eventsScopeMock.Object);

            foreach (var pipelineModuleMock in pipelineModuleMocks)
                pipelineModuleMock.Verify();
        }

        private Mock<IPipelineModule<object>> SetUpPipelineModule(int index)
        {
            var pipelineModuleMock = new Mock<IPipelineModule<object>>(MockBehavior.Strict);
            var module = AddModule(index, pipelineModuleMock);

            pipelineModuleMock
                .Setup(x => x.InvokeAsync(It.IsAny<object>(), It.IsAny<PipelineContext>(), It.IsAny<NextModuleDelegate>()))
                .Callback<object, PipelineContext, NextModuleDelegate>(async (config, context, next) => await next(context))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var loggerType = typeof(ILogger<>).MakeGenericType(module.GetType());
            _internalServiceProviderMock
                .Setup(x => x.GetService(loggerType))
                .Returns(new LoggerFactory().CreateLogger(loggerType))
                .Verifiable();

            return pipelineModuleMock;
        }

        private object AddModule(int index, Mock<IPipelineModule<object>> pipelineModuleMock)
        {
            IPipelineModule<object> module;

            T SetUpServiceProviderAndAddModule<T>(T moduleToAdd) where T : IPipelineModule<object>
            {
                _internalServiceProviderMock
                    .Setup(x => x.GetService(typeof(T)))
                    .Returns(pipelineModuleMock.Object)
                    .Verifiable();

                _pipeline.AddModule<T, object>(new object());
               
                return moduleToAdd;
            }

            switch (index)
            {
                case 0:
                    module = SetUpServiceProviderAndAddModule(new PipelineModule1());
                    break;
                case 1:
                    module = SetUpServiceProviderAndAddModule(new PipelineModule2());
                    break;
                case 2:
                    module = SetUpServiceProviderAndAddModule(new PipelineModule3());
                    break;
                case 3:
                    module = SetUpServiceProviderAndAddModule(new PipelineModule4());
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
