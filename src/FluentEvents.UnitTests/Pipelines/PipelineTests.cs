using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines
{
    [TestFixture]
    public class PipelineTests
    {
        private EventsScope _eventsScope;
        private Mock<IServiceProvider> _internalServiceProviderMock;
        private Pipeline _pipeline;

        [SetUp]
        public void SetUp()
        {
            _eventsScope = new EventsScope();
            _internalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _pipeline = new Pipeline(_internalServiceProviderMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _internalServiceProviderMock.Verify();
        }

        [Test]
        public void AddModuleConfig_ShouldAdd()
        {
            _pipeline.AddModule<PipelineModule1, object>(new object());
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
            await _pipeline.ProcessEventAsync(new PipelineEvent(
                    typeof(object),
                    "f",
                    new object(),
                    new object()
                ),
                _eventsScope
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

            await _pipeline.ProcessEventAsync(new PipelineEvent(
                    typeof(object),
                    "f",
                    new object(),
                    new object()
                ),
                _eventsScope
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
            _internalServiceProviderMock
                .Setup(x => x.GetService(loggerType))
                .Returns(new LoggerFactory().CreateLogger(loggerType))
                .Verifiable();

            _internalServiceProviderMock
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
                    _pipeline.AddModule<PipelineModule1, object>(new object());
                    break;
                case 1:
                    module = new PipelineModule2();
                    _pipeline.AddModule<PipelineModule2, object>(new object());
                    break;
                case 2:
                    module = new PipelineModule3();
                    _pipeline.AddModule<PipelineModule3, object>(new object());
                    break;
                case 3:
                    module = new PipelineModule4();
                    _pipeline.AddModule<PipelineModule4, object>(new object());
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
