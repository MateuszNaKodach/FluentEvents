using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Filters;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Filters
{
    [TestFixture]
    public class FilterPipelineModuleTests : PipelineModuleTestBase
    {
        private FilterPipelineModule _filterPipelineModule;
        private FilterPipelineModuleConfig _filterPipelineModuleConfig;

        [SetUp]
        public void SetUp()
        {
            _filterPipelineModule = new FilterPipelineModule();
            _filterPipelineModuleConfig = new FilterPipelineModuleConfig((sender, args) => ((TestSender)sender).IsValid);
        }

        [Test]
        public async Task InvokeAsync_WithoutMatch_ShouldNotInvokeNextModule()
        {
            var testSender = new TestSender { IsValid = false };
            var testEventArgs = new TestEventArgs();

            var pipelineContext = CreatePipelineContext(
                testSender,
                testEventArgs
            );

            var isInvoked = false;

            Task InvokeNextModule(PipelineContext context)
            {
                isInvoked = true;
                return Task.CompletedTask;
            }

            await _filterPipelineModule.InvokeAsync(_filterPipelineModuleConfig, pipelineContext, InvokeNextModule);

            Assert.That(isInvoked, Is.False);
        }

        [Test]
        public async Task InvokeAsync_WithMatch_ShouldInvokeNextModule()
        {
            var testSender = new TestSender { IsValid = true };
            var testEventArgs = new TestEventArgs();

            var pipelineContext = CreatePipelineContext(
                testSender,
                testEventArgs
            );

            PipelineContext nextModuleContext = null;

            Task InvokeNextModule(PipelineContext context)
            {
                nextModuleContext = context;
                return Task.CompletedTask;
            }

            await _filterPipelineModule.InvokeAsync(_filterPipelineModuleConfig, pipelineContext, InvokeNextModule);

            Assert.That(nextModuleContext, Is.Not.Null);
            Assert.That(nextModuleContext, Is.EqualTo(pipelineContext));
        }
        
        private class TestSender
        {
            public bool IsValid { get; set; }
        }

        private class TestEventArgs
        {

        }
    }
}
