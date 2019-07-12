using System;
using FluentEvents.Pipelines.Projections;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Projections
{
    [TestFixture]
    public class EventProjectionTests
    {
        private Func<object, object> _conversionFunc;
        private object _convertedEventArgs;
        private EventProjection<object, object> _eventProjection;

        [SetUp]
        public void SetUp()
        {
            _convertedEventArgs = new object();
            _conversionFunc = eventArgs => _convertedEventArgs;
            _eventProjection = new EventProjection<object, object>(_conversionFunc);
        }

        [Test]
        public void Convert_ShouldReturnConvertedEventArgs([Values] bool areEventArgsNull)
        {
            var convertedEventArgs = _eventProjection.Convert(areEventArgsNull ? null : new object());

            Assert.That(convertedEventArgs, Is.EqualTo(_convertedEventArgs));
        }
    }
}
