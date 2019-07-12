using FluentEvents.Pipelines;
using FluentEvents.Transmission;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Transmission
{
    [TestFixture]
    public class JsonEventsSerializationServiceTests
    {
        private JsonEventsSerializationService _jsonEventsSerializationService;
        private PipelineEvent _pipelineEvent;
        private TestEvent _originalEvent;

        [SetUp]
        public void SetUp()
        {
            _originalEvent = new TestEvent
            {
                Property1 = 20
            };
            _pipelineEvent = new PipelineEvent(_originalEvent);

            _jsonEventsSerializationService = new JsonEventsSerializationService();
        }

        [Test]
        public void SerializeEvent_ShouldProduceJsonDeserializableWith_DeserializeEvent()
        {
            var json = _jsonEventsSerializationService.SerializeEvent(_pipelineEvent);

            var deserializedPipelineEvent = _jsonEventsSerializationService.DeserializeEvent(json);

            Assert.That(
                deserializedPipelineEvent,
                Has.Property(nameof(PipelineEvent.Event))
                    .With.Property(nameof(TestEvent.Property1))
                    .EqualTo(_originalEvent.Property1)
            );
        }
        
        private class TestEvent
        {
            public int Property1 { get; set; }
        }
    }
}
