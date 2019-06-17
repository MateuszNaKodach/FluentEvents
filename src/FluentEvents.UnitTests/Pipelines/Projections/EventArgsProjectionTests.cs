using System;
using FluentEvents.Pipelines.Projections;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Projections
{
    [TestFixture]
    public class EventArgsProjectionTests
    {
        private Func<object, object> _conversionFunc;
        private object _convertedEventArgs;
        private EventArgsProjection<object, object> _eventArgsProjection;

        [SetUp]
        public void SetUp()
        {
            _convertedEventArgs = new object();
            _conversionFunc = eventArgs => _convertedEventArgs;
            _eventArgsProjection = new EventArgsProjection<object, object>(_conversionFunc);
        }

        [Test]
        public void Convert_ShouldReturnConvertedEventArgs([Values] bool areEventArgsNull)
        {
            var convertedEventArgs = _eventArgsProjection.Convert(areEventArgsNull ? null : new object());

            Assert.That(convertedEventArgs, Is.EqualTo(_convertedEventArgs));
        }
    }
}
