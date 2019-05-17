using System;
using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsQueueNamesServiceTests
    {
        private const string QueueName = nameof(QueueName);

        private EventsQueueNamesService _eventsQueueNamesService;

        [SetUp]
        public void SetUp()
        {
            _eventsQueueNamesService = new EventsQueueNamesService();
        }

        [Test]
        public void RegisterQueueNameIfNotExists_ShouldAddNameOnce()
        {
            _eventsQueueNamesService.RegisterQueueNameIfNotExists(QueueName);
            _eventsQueueNamesService.RegisterQueueNameIfNotExists(QueueName);

            var exists = _eventsQueueNamesService.IsQueueNameExisting(QueueName);

            Assert.That(exists, Is.True);
        }

        [Test]
        public void RegisterQueueNameIfNotExists_WithNullQueueName_ShouldThrow()
        {
            Assert.That(() =>
            {
                _eventsQueueNamesService.RegisterQueueNameIfNotExists(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void IsQueueNameExisting_WhenQueueIsNotRegistered_ShouldReturnFalse()
        {
            var exists = _eventsQueueNamesService.IsQueueNameExisting(QueueName);

            Assert.That(exists, Is.False);
        }

        [Test]
        public void IsQueueNameExisting_WhenQueueIsRegistered_ShouldReturnTrue()
        {
            _eventsQueueNamesService.RegisterQueueNameIfNotExists(QueueName);

            var exists = _eventsQueueNamesService.IsQueueNameExisting(QueueName);

            Assert.That(exists, Is.True);
        }
    }
}
