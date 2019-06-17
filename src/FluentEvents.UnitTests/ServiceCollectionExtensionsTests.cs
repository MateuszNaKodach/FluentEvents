using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
        private IServiceCollection _serviceCollection;

        [SetUp]
        public void SetUp()
        {
            _serviceCollection = new ServiceCollection();
        }

        [Test]
        public void AddEventsContext_ShouldInvokeOptionsActionAndAddServices()
        {
            var isOptionsActionInvoked = false;
            var serviceCollection = _serviceCollection.AddEventsContext<TestEventsContext>(options =>
            {
                isOptionsActionInvoked = true;
            });

            Assert.That(isOptionsActionInvoked, Is.True);
            Assert.That(
                _serviceCollection,
                Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(EventsScope))
            );
            Assert.That(
                _serviceCollection,
                Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(EventsContext))
            );
            Assert.That(
                _serviceCollection,
                Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(TestEventsContext))
            );
            Assert.That(
                _serviceCollection,
                Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(IHostedService))
            );
            Assert.That(serviceCollection, Is.EqualTo(_serviceCollection));
        }

        [Test]
        public void AddWithEventsAttachedTo_ShouldReplaceServiceDescriptorsWithFactoryThatAttachesTheService(
            [Values] ServiceDescriptorImplementation serviceDescriptorImplementation
        )
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

            _serviceCollection.AddWithEventsAttachedTo<TestEventsContext>(() =>
            {
                switch (serviceDescriptorImplementation)
                {
                    case ServiceDescriptorImplementation.Type:
                        _serviceCollection.AddSingleton<TestService1>();
                        break;
                    case ServiceDescriptorImplementation.Factory:
                        _serviceCollection.AddSingleton(x => new TestService1());
                        break;
                    case ServiceDescriptorImplementation.Instance:
                        _serviceCollection.AddSingleton(new TestService1());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(serviceDescriptorImplementation),
                            serviceDescriptorImplementation,
                            null
                        );
                }
            });

            var factory = _serviceCollection.First().ImplementationFactory;

            var service = factory(serviceProviderMock.Object);

            Assert.That(service, Is.Not.Null);

            serviceProviderMock.Verify();
            eventsContextMock.Verify();
        }

        [Test]
        public void AddWithEventsAttachedTo_WithUnsupportedImplementationType_ShouldThrow()
        {
            var constructor = typeof(ServiceDescriptor).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] {typeof(Type), typeof(ServiceLifetime)},
                null
            );

            var serviceDescriptor = (ServiceDescriptor) constructor.Invoke(new object[]
            {
                typeof(object),
                ServiceLifetime.Scoped
            });

            Assert.That(() =>
            {
                _serviceCollection.AddWithEventsAttachedTo<TestEventsContext>(() =>
                {
                    _serviceCollection.Add(serviceDescriptor);
                });
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void AddWithEventsAttachedTo_WithGenericTypeDefinition_ShouldNotReplaceServiceDescriptor()
        {
            var serviceDescriptor = new ServiceDescriptor(typeof(TestService3<>), new TestService3<object>());

            _serviceCollection.AddWithEventsAttachedTo<TestEventsContext>(() =>
            {
                _serviceCollection.Add(serviceDescriptor);
            });

            Assert.That(_serviceCollection, Has.One.Items.EqualTo(serviceDescriptor));
        }

        [Test]
        public void AddWithEventsAttachedTo_ShouldReplaceOnlyCorrectServiceDescriptorsWithCustomFactory(
            [Values] ServiceLifetime serviceLifetime
        )
        {
            var serviceDescriptor0 = new ServiceDescriptor(typeof(TestService0), new TestService0());
            _serviceCollection.Add(serviceDescriptor0);

            _serviceCollection.AddWithEventsAttachedTo<TestEventsContext>(() =>
            {
                _serviceCollection.Add(
                    new ServiceDescriptor(typeof(TestService1), typeof(TestService1), serviceLifetime)
                );

                _serviceCollection.Add(
                    new ServiceDescriptor(typeof(TestService1), x => new TestService1(), serviceLifetime)
                );

                _serviceCollection.Add(
                    new ServiceDescriptor(typeof(TestService1), new TestService1())
                );
            });

            var serviceDescriptor2 = new ServiceDescriptor(typeof(TestService2), new TestService2());
            _serviceCollection.Add(serviceDescriptor2);

            Assert.That(
                _serviceCollection,
                Has.Exactly(3).Items.With.Property(nameof(ServiceDescriptor.ImplementationFactory)).Not.Null
            );

            var servicesWithCurrentLifetimeCount = serviceLifetime == ServiceLifetime.Singleton ? 3 : 2;
            
            Assert.That(
                _serviceCollection,
                Has.Exactly(servicesWithCurrentLifetimeCount)
                    .Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(TestService1))
                    .And
                    .Property(nameof(ServiceDescriptor.Lifetime)).EqualTo(serviceLifetime)
            );

            if (serviceLifetime != ServiceLifetime.Singleton)
                Assert.That(
                    _serviceCollection,
                    Has.Exactly(1)
                        .Items.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(TestService1))
                        .And
                        .Property(nameof(ServiceDescriptor.Lifetime)).EqualTo(ServiceLifetime.Singleton)
                );

            Assert.That(
                _serviceCollection,
                Has.One.Items.EqualTo(serviceDescriptor0)
            );

            Assert.That(
                _serviceCollection,
                Has.One.Items.EqualTo(serviceDescriptor2)
            );
        }

        public enum ServiceDescriptorImplementation
        {
            Type,
            Factory,
            Instance
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
