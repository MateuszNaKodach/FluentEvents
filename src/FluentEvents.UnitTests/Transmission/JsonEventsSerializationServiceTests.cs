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
        private TestSender _originalSender;
        private TestEventArgs _originalEventArgs;

        [SetUp]
        public void SetUp()
        {
            _originalSender = new TestSender
            {
                Property1 = 10
            };
            _originalEventArgs = new TestEventArgs
            {
                Property1 = 20
            };
            _pipelineEvent = new PipelineEvent(
                typeof(TestSender),
                "f",
                _originalSender,
                _originalEventArgs
            );

            _jsonEventsSerializationService = new JsonEventsSerializationService();
        }

        [Test]
        public void SerializeEvent_ShouldProduceJsonDeserializableWith_DeserializeEvent()
        {
            var json = _jsonEventsSerializationService.SerializeEvent(_pipelineEvent);

            var deserializedPipelineEvent = _jsonEventsSerializationService.DeserializeEvent(json);

            Assert.That(
                deserializedPipelineEvent,
                Has.Property(nameof(PipelineEvent.OriginalEventFieldName))
                    .EqualTo(_pipelineEvent.OriginalEventFieldName)
            );

            Assert.That(
                deserializedPipelineEvent,
                Has.Property(nameof(PipelineEvent.OriginalSender))
                    .With.Property(nameof(TestSender.Property1))
                    .EqualTo(_originalSender.Property1)
            );

            Assert.That(
                deserializedPipelineEvent,
                Has.Property(nameof(PipelineEvent.OriginalEventArgs))
                    .With.Property(nameof(TestSender.Property1))
                    .EqualTo(_originalEventArgs.Property1)
            );
        }

        private class TestSender
        {
            public int Property1 { get; set; }
        }

        private class TestEventArgs
        {
            public int Property1 { get; set; }
        }
    }
}
