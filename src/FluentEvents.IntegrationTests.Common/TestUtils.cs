using FluentEvents.Infrastructure;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests.Common
{
    public static class TestUtils
    {
        private static readonly int _defaultTestEntityId = 5;
        private static readonly string _defaultTestEventArgsValue = nameof(_defaultTestEventArgsValue);

        public static TestEntity AttachAndRaiseEvent(EventsContext eventsContext, EventsScope eventsScope)
        {
            var entity = new TestEntity
            {
                Id = _defaultTestEntityId
            };

            eventsContext.Attach(entity, eventsScope);

            entity.RaiseEvent(_defaultTestEventArgsValue);

            return entity;
        }

        public static void AssertThatEventIsPublishedProperly(object e)
        {
            Assert.That(e, Is.Not.Null);
            Assert.That(
                e,
                Has.Property(nameof(TestEvent.Value)).EqualTo(_defaultTestEventArgsValue)
            );
        }
    }
}
