using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsQueuesContextTests
    {
        private EventsQueuesContext _eventsQueuesContext;

        [SetUp]
        public void SetUp()
        {
            _eventsQueuesContext = new EventsQueuesContext();
        }

        [Test]
        public void Guid_ShouldGenerateNewGuidOnce()
        {
            var guid1 = _eventsQueuesContext.Guid;
            var guid2 = _eventsQueuesContext.Guid;

            Assert.That(guid1, Is.EqualTo(guid2));
        }
    }
}
