using System;
using FluentEvents.Pipelines.Projections;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Projections
{
    [TestFixture]
    public class EventSenderProjectionTests
    {
        private Func<object, object> _conversionFunc;
        private object _convertedEventSender;
        private EventSenderProjection<object, object> _eventSenderProjection;

        [SetUp]
        public void SetUp()
        {
            _convertedEventSender = new object();
            _conversionFunc = eventSender => _convertedEventSender;
            _eventSenderProjection = new EventSenderProjection<object, object>(_conversionFunc);
        }

        [Test]
        public void Convert_ShouldReturnConvertedEventSender([Values] bool areEventSenderNull)
        {
            var convertedEventSender = _eventSenderProjection.Convert(areEventSenderNull ? null : new object());

            Assert.That(convertedEventSender, Is.EqualTo(_convertedEventSender));
        }
    }
}
