using FluentEvents.Pipelines;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines
{
    [TestFixture]
    public class PipelinesServiceTests
    {
        private Mock<IPipeline> _pipelineMock;

        private PipelinesService _pipelinesService;

        [SetUp]
        public void SetUp()
        {
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);

            _pipelinesService = new PipelinesService();
        }

        [Test]
        public void AddPipeline_ShouldAdd()
        {
            _pipelinesService.AddPipeline(typeof(object), _pipelineMock.Object);

            var pipelines = _pipelinesService.GetPipelines(typeof(object));

            Assert.That(pipelines, Has.One.Items);
        }

        [Test]
        public void AddPipeline_TwiceWithSameType_ShouldAdd()
        {
            _pipelinesService.AddPipeline(typeof(object), _pipelineMock.Object);
            _pipelinesService.AddPipeline(typeof(object), _pipelineMock.Object);

            var pipelines = _pipelinesService.GetPipelines(typeof(object));

            Assert.That(pipelines, Has.Exactly(2).Items);
        }

        [Test]
        public void GetPipelines_WithEventImplementingInterface_ShouldReturnPipelinesWithThatInterface()
        {
            _pipelinesService.AddPipeline(typeof(Event4), _pipelineMock.Object);
            _pipelinesService.AddPipeline(typeof(IEvent), _pipelineMock.Object);

            var pipelines = _pipelinesService.GetPipelines(typeof(Event3));

            Assert.That(pipelines, Has.One.Items);
        }

        [Test]
        public void GetPipelines_WithEventBaseTypeImplementingInterface_ShouldReturnPipelinesWithThatInterfaceOrBaseTypes()
        {
            SetUpAllPipelines();

            var pipelines = _pipelinesService.GetPipelines(typeof(Event4));

            Assert.That(pipelines, Has.Exactly(5).Items);
        }

        [Test]
        public void GetPipelines_WithEventImplementingInterface_ShouldReturnPipelinesWithThatInterfaceOrBaseTypes()
        {
            SetUpAllPipelines();

            var pipelines = _pipelinesService.GetPipelines(typeof(Event3));

            Assert.That(pipelines, Has.Exactly(4).Items);
        }

        [Test]
        public void GetPipelines_WithEventBaseType_ShouldReturnPipelinesWithBaseTypes()
        {
            SetUpAllPipelines();

            var pipelines = _pipelinesService.GetPipelines(typeof(Event2));

            Assert.That(pipelines, Has.Exactly(2).Items);
        }

        private void SetUpAllPipelines()
        {
            _pipelinesService.AddPipeline(typeof(Event1), _pipelineMock.Object);
            _pipelinesService.AddPipeline(typeof(Event2), _pipelineMock.Object);
            _pipelinesService.AddPipeline(typeof(Event3), _pipelineMock.Object);
            _pipelinesService.AddPipeline(typeof(Event4), _pipelineMock.Object);
            _pipelinesService.AddPipeline(typeof(Event5), _pipelineMock.Object);
            _pipelinesService.AddPipeline(typeof(IEvent), _pipelineMock.Object);
        }

        private interface IEvent
        {
        }

        private class Event1
        {
            
        }

        private class Event2 : Event1
        {

        }

        private class Event3 : Event2, IEvent
        {

        }

        private class Event4 : Event3
        {

        }

        private class Event5 : Event4
        {

        }
    }
}
