using System;
using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsQueueNamesServiceTests
    {
        private const string QueueName = nameof(QueueName);

        private EventsQueueNamesService m_EventsQueueNamesService;

        [SetUp]
        public void SetUp()
        {
            m_EventsQueueNamesService = new EventsQueueNamesService();
        }

        [Test]
        public void RegisterQueueNameIfNotExists_ShouldAddNameOnce()
        {
            m_EventsQueueNamesService.RegisterQueueNameIfNotExists(QueueName);
            m_EventsQueueNamesService.RegisterQueueNameIfNotExists(QueueName);

            var exists = m_EventsQueueNamesService.IsQueueNameExisting(QueueName);

            Assert.That(exists, Is.True);
        }

        [Test]
        public void RegisterQueueNameIfNotExists_WithNullQueueName_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_EventsQueueNamesService.RegisterQueueNameIfNotExists(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void IsQueueNameExisting_WhenQueueIsNotRegistered_ShouldReturnFalse()
        {
            var exists = m_EventsQueueNamesService.IsQueueNameExisting(QueueName);

            Assert.That(exists, Is.False);
        }

        [Test]
        public void IsQueueNameExisting_WhenQueueIsRegistered_ShouldReturnTrue()
        {
            m_EventsQueueNamesService.RegisterQueueNameIfNotExists(QueueName);

            var exists = m_EventsQueueNamesService.IsQueueNameExisting(QueueName);

            Assert.That(exists, Is.True);
        }
    }
}
