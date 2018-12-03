using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AsyncEvent;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Model
{
    [TestFixture]
    public class SourceModelEventFieldTests
    {
        private EventsContextImpl m_EventsContext;
        private SourceModel m_SourceModel;
        private SourceModelEventField m_SourceModelEventField;
        private SourceModelEventField m_AsyncSourceModelEventField;
        private SourceModelEventField m_InheritedSourceModelEventField;

        [SetUp]
        public void SetUp()
        {
            m_EventsContext = new EventsContextImpl();
            m_SourceModel = new SourceModel(typeof(TestModel), m_EventsContext);
            m_SourceModelEventField = m_SourceModel.GetOrCreateEventField(nameof(TestModel.TestEvent));
            m_AsyncSourceModelEventField = m_SourceModel.GetOrCreateEventField(nameof(TestModel.AsyncTestEvent));
            m_InheritedSourceModelEventField = m_SourceModel.GetOrCreateEventField(nameof(TestModel.InheritedTestEvent));
        }

        private SourceModelEventField GetSourceModelEventField(string name)
        {
            switch (name)
            {
                case nameof(m_SourceModelEventField):
                    return m_SourceModelEventField;
                case nameof(m_AsyncSourceModelEventField):
                    return m_AsyncSourceModelEventField;
                case nameof(m_InheritedSourceModelEventField):
                    return m_InheritedSourceModelEventField;
                default:
                    throw new ArgumentOutOfRangeException(nameof(name));
            }
        }

        [TestCase(nameof(m_SourceModelEventField))]
        [TestCase(nameof(m_AsyncSourceModelEventField))]
        [TestCase(nameof(m_InheritedSourceModelEventField))]
        public void IsAsync_ShouldReturnTrueWhenReturnTypeIsTask(string eventFieldName)
        {
            var eventField = GetSourceModelEventField(eventFieldName);
            var isAsyncField = eventFieldName == nameof(m_AsyncSourceModelEventField);

            Assert.That(eventField, Has.Property(nameof(SourceModelEventField.IsAsync)).EqualTo(isAsyncField));
        }

        [TestCase(nameof(m_SourceModelEventField))]
        [TestCase(nameof(m_AsyncSourceModelEventField))]
        [TestCase(nameof(m_InheritedSourceModelEventField))]
        public void ReturnType_ShouldReturnFieldReturnType(string eventFieldName)
        {
            var eventField = GetSourceModelEventField(eventFieldName);

            var isAsyncField = eventFieldName == nameof(m_AsyncSourceModelEventField);
            var expectedReturnType = isAsyncField ? typeof(Task) : typeof(void);

            Assert.That(eventField, Has.Property(nameof(SourceModelEventField.ReturnType)).EqualTo(expectedReturnType));
        }

        [TestCase(nameof(m_SourceModelEventField))]
        [TestCase(nameof(m_AsyncSourceModelEventField))]
        [TestCase(nameof(m_InheritedSourceModelEventField))]
        public void EventHandlerParameters_ShouldReturnFieldEventHandlerParameters(string eventFieldName)
        {
            var eventField = GetSourceModelEventField(eventFieldName);

            Assert.That(
                eventField, 
                Has.Property(nameof(SourceModelEventField.EventHandlerParameters))
                    .With.One.Items.With.Property(nameof(ParameterExpression.Type))
                    .EqualTo(typeof(TestEventArgs))
            );

            Assert.That(
                eventField,
                Has.Property(nameof(SourceModelEventField.EventHandlerParameters))
                    .With.One.Items.With.Property(nameof(ParameterExpression.Type))
                    .EqualTo(typeof(object))
            );
        }

        [Test]
        public void EventInfo_ShouldReturnFieldEventInfo()
        {
            var expectedEventInfo = typeof(TestModel).GetEvent(nameof(TestModel.TestEvent));

            Assert.That(
                m_SourceModelEventField,
                Has.Property(nameof(SourceModelEventField.EventInfo)).EqualTo(expectedEventInfo)
            );
        }


        [Test]
        public void FieldInfo_ShouldReturnFieldInfo()
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;
            var expectedFieldInfo = typeof(TestModel).GetField(nameof(TestModel.TestEvent), bindingFlags);

            Assert.That(
                m_SourceModelEventField,
                Has.Property(nameof(SourceModelEventField.FieldInfo)).EqualTo(expectedFieldInfo)
            );
        }

        [Test]
        public void Name_ShouldReturnFieldInfoName()
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;
            var expectedFieldInfo = typeof(TestModel).GetField(nameof(TestModel.TestEvent), bindingFlags);

            Assert.That(
                m_SourceModelEventField,
                Has.Property(nameof(SourceModelEventField.Name)).EqualTo(expectedFieldInfo.Name)
            );
        }

        [Test]
        public void AddEventPipelineConfig_ShouldCreateAndAddNewPipeline([Values(null, "q")] string queueName)
        {
            var pipeline = m_SourceModelEventField.AddEventPipelineConfig(queueName);

            Assert.That(pipeline, Has.Property(nameof(Pipeline.EventsContext)).EqualTo(m_EventsContext));
            Assert.That(pipeline, Has.Property(nameof(Pipeline.QueueName)).EqualTo(queueName));
            Assert.That(m_SourceModelEventField.Pipelines, Has.One.Items.EqualTo(pipeline));
        }

        private class TestModel : TestModelBase
        {
            public event EventHandler<TestEventArgs> TestEvent;
            public event AsyncEventHandler<TestEventArgs> AsyncTestEvent;
        }

        private class TestModelBase
        {
            public event EventHandler<TestEventArgs> InheritedTestEvent;
        }

        private class TestEventArgs
        {
        }
    }
}
