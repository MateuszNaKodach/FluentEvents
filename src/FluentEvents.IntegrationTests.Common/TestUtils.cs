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

        public static void AssertThatEventIsPublishedProperly(object sender, TestEventArgs eventArgs)
        {
            Assert.That(sender, Is.Not.Null);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(sender, Is.TypeOf<TestEntity>());
            Assert.That(
                sender,
                Has.Property(nameof(TestEntity.Id)).EqualTo(TestUtils._defaultTestEntityId)
            );
            Assert.That(
                eventArgs,
                Has.Property(nameof(TestEventArgs.Value)).EqualTo(TestUtils._defaultTestEventArgsValue)
            );
        }
    }
}
