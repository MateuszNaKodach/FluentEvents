using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AsyncEvent;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Model
{
    [TestFixture]
    public class SourceModelEventFieldTests
    {
        private Mock<IPipeline> _pipelineMock;
        private SourceModel _sourceModel;
        private SourceModelEventField _sourceModelEventField;
        private SourceModelEventField _asyncSourceModelEventField;
        private SourceModelEventField _inheritedSourceModelEventField;

        [SetUp]
        public void SetUp()
        {
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            _sourceModel = new SourceModel(typeof(TestModel));
            _sourceModelEventField = _sourceModel.GetOrCreateEventField(nameof(TestModel.TestEvent));
            _asyncSourceModelEventField = _sourceModel.GetOrCreateEventField(nameof(TestModel.AsyncTestEvent));
            _inheritedSourceModelEventField = _sourceModel.GetOrCreateEventField(nameof(TestModel.InheritedTestEvent));
        }

        [TearDown]
        public void TearDown()
        {
            _pipelineMock.Verify();
        }

        private SourceModelEventField GetSourceModelEventField(string name)
        {
            switch (name)
            {
                case nameof(_sourceModelEventField):
                    return _sourceModelEventField;
                case nameof(_asyncSourceModelEventField):
                    return _asyncSourceModelEventField;
                case nameof(_inheritedSourceModelEventField):
                    return _inheritedSourceModelEventField;
                default:
                    throw new ArgumentOutOfRangeException(nameof(name));
            }
        }

        [TestCase(nameof(_sourceModelEventField))]
        [TestCase(nameof(_asyncSourceModelEventField))]
        [TestCase(nameof(_inheritedSourceModelEventField))]
        public void IsAsync_ShouldReturnTrueWhenReturnTypeIsTask(string eventFieldName)
        {
            var eventField = GetSourceModelEventField(eventFieldName);
            var isAsyncField = eventFieldName == nameof(_asyncSourceModelEventField);

            Assert.That(eventField, Has.Property(nameof(SourceModelEventField.IsAsync)).EqualTo(isAsyncField));
        }

        [TestCase(nameof(_sourceModelEventField))]
        [TestCase(nameof(_asyncSourceModelEventField))]
        [TestCase(nameof(_inheritedSourceModelEventField))]
        public void ReturnType_ShouldReturnFieldReturnType(string eventFieldName)
        {
            var eventField = GetSourceModelEventField(eventFieldName);

            var isAsyncField = eventFieldName == nameof(_asyncSourceModelEventField);
            var expectedReturnType = isAsyncField ? typeof(Task) : typeof(void);

            Assert.That(eventField, Has.Property(nameof(SourceModelEventField.ReturnType)).EqualTo(expectedReturnType));
        }

        [TestCase(nameof(_sourceModelEventField))]
        [TestCase(nameof(_asyncSourceModelEventField))]
        [TestCase(nameof(_inheritedSourceModelEventField))]
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
                _sourceModelEventField,
                Has.Property(nameof(SourceModelEventField.EventInfo)).EqualTo(expectedEventInfo)
            );
        }


        [Test]
        public void FieldInfo_ShouldReturnFieldInfo()
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;
            var expectedFieldInfo = typeof(TestModel).GetField(nameof(TestModel.TestEvent), bindingFlags);

            Assert.That(
                _sourceModelEventField,
                Has.Property(nameof(SourceModelEventField.FieldInfo)).EqualTo(expectedFieldInfo)
            );
        }

        [Test]
        public void Name_ShouldReturnFieldInfoName()
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;
            var expectedFieldInfo = typeof(TestModel).GetField(nameof(TestModel.TestEvent), bindingFlags);

            Assert.That(
                _sourceModelEventField,
                Has.Property(nameof(SourceModelEventField.Name)).EqualTo(expectedFieldInfo.Name)
            );
        }

        [Test]
        public void AddPipeline_ShouldAddNewPipeline()
        {
            var pipeline = _sourceModelEventField.AddPipeline(_pipelineMock.Object);

            Assert.That(_sourceModelEventField.Pipelines, Has.One.Items.EqualTo(pipeline));
        }

        [Test]
        public void AddPipeline_WithNullPipeline_ShouldThrow()
        {
            Assert.That(() =>
            {
                _sourceModelEventField.AddPipeline(null);
            }, Throws.TypeOf<ArgumentNullException>());
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
