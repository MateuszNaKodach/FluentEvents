using FluentEvents.Pipelines;
using FluentEvents.Transmission;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Transmission
{
    [TestFixture]
    public class JsonEventsSerializationServiceTests
    {
        private JsonEventsSerializationService m_JsonEventsSerializationService;
        private PipelineEvent m_PipelineEvent;
        private TestSender m_OriginalSender;
        private TestEventArgs m_OriginalEventArgs;

        [SetUp]
        public void SetUp()
        {
            m_OriginalSender = new TestSender
            {
                Property1 = 10
            };
            m_OriginalEventArgs = new TestEventArgs
            {
                Property1 = 20
            };
            m_PipelineEvent = new PipelineEvent(
                typeof(TestSender),
                "f",
                m_OriginalSender,
                m_OriginalEventArgs
            );

            m_JsonEventsSerializationService = new JsonEventsSerializationService();
        }

        [Test]
        public void SerializeEvent_ShouldProduceJsonDeserializableWith_DeserializeEvent()
        {
            var json = m_JsonEventsSerializationService.SerializeEvent(m_PipelineEvent);

            var deserializedPipelineEvent = m_JsonEventsSerializationService.DeserializeEvent(json);

            Assert.That(
                deserializedPipelineEvent,
                Has.Property(nameof(PipelineEvent.OriginalEventFieldName))
                    .EqualTo(m_PipelineEvent.OriginalEventFieldName)
            );

            Assert.That(
                deserializedPipelineEvent,
                Has.Property(nameof(PipelineEvent.OriginalSender))
                    .With.Property(nameof(TestSender.Property1))
                    .EqualTo(m_OriginalSender.Property1)
            );

            Assert.That(
                deserializedPipelineEvent,
                Has.Property(nameof(PipelineEvent.OriginalEventArgs))
                    .With.Property(nameof(TestSender.Property1))
                    .EqualTo(m_OriginalEventArgs.Property1)
            );
        }

        public class TestSender
        {
            public int Property1 { get; set; }
        }

        public class TestEventArgs
        {
            public int Property1 { get; set; }
        }
    }
}
