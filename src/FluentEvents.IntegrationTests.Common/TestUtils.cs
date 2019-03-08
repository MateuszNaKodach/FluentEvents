using NUnit.Framework;

namespace FluentEvents.IntegrationTests.Common
{
    public static class TestUtils
    {
        private static readonly int DefaultTestEntityId = 5;
        private static readonly string DefaultTestEventArgsValue = nameof(DefaultTestEventArgsValue);

        public static TestEntity AttachAndRaiseEvent(EventsContext eventsContext, EventsScope eventsScope)
        {
            var entity = new TestEntity
            {
                Id = DefaultTestEntityId
            };

            eventsContext.Attach(entity, eventsScope);

            entity.RaiseEvent(DefaultTestEventArgsValue);

            return entity;
        }

        public static void AssertThatEventIsPublishedProperly(object sender, TestEventArgs eventArgs)
        {
            Assert.That(sender, Is.Not.Null);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(sender, Is.TypeOf<TestEntity>());
            Assert.That(
                sender,
                Has.Property(nameof(TestEntity.Id)).EqualTo(TestUtils.DefaultTestEntityId)
            );
            Assert.That(
                eventArgs,
                Has.Property(nameof(TestEventArgs.Value)).EqualTo(TestUtils.DefaultTestEventArgsValue)
            );
        }
    }
}
