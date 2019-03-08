using System;
using System.Linq;
using FluentEvents.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests
{
    [TestFixture]
    public class ServiceCollectionExtensionsTests
    {
        private IServiceCollection m_ServiceCollection;

        [SetUp]
        public void SetUp()
        {
            m_ServiceCollection = new ServiceCollection();
        }

        [Test]
        public void AddEventsContext_ShouldInvokeOptionsActionAndAddServices()
        {
            var isOptionsActionInvoked = false;
            var serviceCollection = m_ServiceCollection.AddEventsContext<TestEventsContext>(options =>
            {
                isOptionsActionInvoked = true;
            });

            Assert.That(isOptionsActionInvoked, Is.True);
            Assert.That(
                m_ServiceCollection,
                Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(EventsScope))
            );
            Assert.That(
                m_ServiceCollection,
                Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(EventsContext))
            );
            Assert.That(
                m_ServiceCollection,
                Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(TestEventsContext))
            );
            Assert.That(
                m_ServiceCollection,
                Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(IHostedService))
            );
            Assert.That(serviceCollection, Is.EqualTo(m_ServiceCollection));
        }

        [Test]
        public void AddWithEventsAttachedTo_ShouldReplaceServiceDescriptorsWithFactoryThatAttachesTheService()
        {
            var serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            var eventsContextMock = new Mock<TestEventsContext>(MockBehavior.Strict);
            var eventsScope = new EventsScope();

            eventsContextMock
                .Setup(x => x.Attach(It.IsAny<TestService1>(), eventsScope))
                .Verifiable();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(TestEventsContext)))
                .Returns(eventsContextMock.Object)
                .Verifiable();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(EventsScope)))
                .Returns(eventsScope)
                .Verifiable();

            m_ServiceCollection.AddWithEventsAttachedTo<TestEventsContext>(() =>
            {
                m_ServiceCollection.AddSingleton<TestService1>();
            });

            var factory = m_ServiceCollection.First().ImplementationFactory;

            var service = factory(serviceProviderMock.Object);

            Assert.That(service, Is.Not.Null);

            serviceProviderMock.Verify();
            eventsContextMock.Verify();
        }

        [Test]
        public void AddWithEventsAttachedTo_WithGenericTypeDefinition_ShouldNotReplaceServiceDescriptor()
        {
            var serviceDescriptor = new ServiceDescriptor(typeof(TestService3<>), new TestService3<object>());

            m_ServiceCollection.AddWithEventsAttachedTo<TestEventsContext>(() =>
            {
                m_ServiceCollection.Add(serviceDescriptor);
            });

            Assert.That(m_ServiceCollection, Has.One.Items.EqualTo(serviceDescriptor));
        }

        [Test]
        public void AddWithEventsAttachedTo_ShouldReplaceOnlyCorrectServiceDescriptorsWithCustomFactory(
            [Values] ServiceLifetime serviceLifetime
        )
        {
            var serviceDescriptor0 = new ServiceDescriptor(typeof(TestService0), new TestService0());
            m_ServiceCollection.Add(serviceDescriptor0);

            m_ServiceCollection.AddWithEventsAttachedTo<TestEventsContext>(() =>
            {
                m_ServiceCollection.Add(
                    new ServiceDescriptor(typeof(TestService1), typeof(TestService1), serviceLifetime)
                );

                m_ServiceCollection.Add(
                    new ServiceDescriptor(typeof(TestService1), x => new TestService1(), serviceLifetime)
                );

                m_ServiceCollection.Add(
                    new ServiceDescriptor(typeof(TestService1), new TestService1())
                );
            });

            var serviceDescriptor2 = new ServiceDescriptor(typeof(TestService2), new TestService2());
            m_ServiceCollection.Add(serviceDescriptor2);

            Assert.That(
                m_ServiceCollection,
                Has.Exactly(3).Items.With.Property(nameof(ServiceDescriptor.ImplementationFactory)).Not.Null
            );

            var servicesWithCurrentLifetimeCount = serviceLifetime == ServiceLifetime.Singleton ? 3 : 2;
            
            Assert.That(
                m_ServiceCollection,
                Has.Exactly(servicesWithCurrentLifetimeCount)
                    .Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(TestService1))
                    .And
                    .Property(nameof(ServiceDescriptor.Lifetime)).EqualTo(serviceLifetime)
            );

            if (serviceLifetime != ServiceLifetime.Singleton)
                Assert.That(
                    m_ServiceCollection,
                    Has.Exactly(1)
                        .Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(TestService1))
                        .And
                        .Property(nameof(ServiceDescriptor.Lifetime)).EqualTo(ServiceLifetime.Singleton)
                );

            Assert.That(
                m_ServiceCollection,
                Has.One.Items.EqualTo(serviceDescriptor0)
            );

            Assert.That(
                m_ServiceCollection,
                Has.One.Items.EqualTo(serviceDescriptor2)
            );
        }

        private class TestService0 { }
        private class TestService1 { }
        private class TestService2 { }
        private class TestService3<T> { }

        public class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
            }
        }
    }
}
