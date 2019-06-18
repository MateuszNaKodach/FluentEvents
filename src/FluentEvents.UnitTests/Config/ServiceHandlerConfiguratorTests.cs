using System;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Model;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class ServiceHandlerConfiguratorTests
    {
        private readonly string _event1Name = nameof(SourceEntity.Event);
        private readonly string _event2Name = nameof(SourceEntity.Event2);

        private SourceModel _sourceModel;
        private Mock<IGlobalSubscriptionsService> _globalSubscriptionsServiceMock;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock;
        private Mock<IEventSelectionService> _eventSelectionServiceMock;
        private Mock<ISourceModelsService> _sourceModelsServiceMock;

        private ServiceHandlerConfigurator<SubscribingService, SourceEntity, object> _serviceHandlerConfigurator;

        [SetUp]
        public void SetUp()
        {
            _globalSubscriptionsServiceMock = new Mock<IGlobalSubscriptionsService>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            _eventSelectionServiceMock = new Mock<IEventSelectionService>(MockBehavior.Strict);
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _sourceModel = new SourceModel(typeof(SourceEntity));

            _serviceHandlerConfigurator = new ServiceHandlerConfigurator<SubscribingService, SourceEntity, object>(
                _sourceModel,
                _scopedSubscriptionsServiceMock.Object,
                _globalSubscriptionsServiceMock.Object,
                _eventSelectionServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _globalSubscriptionsServiceMock.Verify();
            _scopedSubscriptionsServiceMock.Verify();
            _eventSelectionServiceMock.Verify();
            _sourceModelsServiceMock.Verify();
        }

        [Test]
        public void HasGlobalSubscription_WithValidEventName_ShouldConfigureSubscription()
        {
            _globalSubscriptionsServiceMock
                .Setup(x => x.AddGlobalServiceHandlerSubscription<SubscribingService, SourceEntity, object>(
                        _event1Name
                    )
                )
                .Verifiable();

            _serviceHandlerConfigurator.HasGlobalSubscriptionTo(_event1Name);
        }

        [Test]
        public void HasGlobalSubscription_WithEventSelector_ShouldConfigureSubscription()
        {
            Action<SourceEntity, dynamic> selectionAction = (entity, o) => { };

            _eventSelectionServiceMock
                .Setup(x => x.GetSingleSelectedEventName(_sourceModel, selectionAction))
                .Returns(_event1Name)
                .Verifiable();

            _globalSubscriptionsServiceMock
                .Setup(x => x.AddGlobalServiceHandlerSubscription<SubscribingService, SourceEntity, object>(
                        _event1Name
                    )
                )
                .Verifiable();

            _serviceHandlerConfigurator.HasGlobalSubscriptionTo(selectionAction);
        }

        [Test]
        public void HasGlobalSubscription_WithEventArgsTypeMismatch_ShouldThrow()
        {
            Assert.That(() =>
            {
                _serviceHandlerConfigurator.HasGlobalSubscriptionTo(_event2Name);
            }, Throws.TypeOf<EventArgsTypeMismatchException>());
        }

        [Test]
        public void HasGlobalSubscription_WithEventNotFound_ShouldThrow()
        {
            Assert.That(() =>
            {
                _serviceHandlerConfigurator.HasGlobalSubscriptionTo("");
            }, Throws.TypeOf<EventFieldNotFoundException>());
        }

        [Test]
        public void HasScopedSubscription_WithValidEventName_ShouldConfigureSubscription()
        {
            _scopedSubscriptionsServiceMock
                .Setup(x => x.ConfigureScopedServiceHandlerSubscription<SubscribingService, SourceEntity, object>(
                        _event1Name
                    )
                )
                .Verifiable();

            _serviceHandlerConfigurator.HasScopedSubscriptionTo(_event1Name);
        }

        [Test]
        public void HasScopedSubscription_WithEventSelector_ShouldConfigureSubscription()
        {
            Action<SourceEntity, dynamic> selectionAction = (entity, o) => { };

            _eventSelectionServiceMock
                .Setup(x => x.GetSingleSelectedEventName(_sourceModel, selectionAction))
                .Returns(_event1Name)
                .Verifiable();

            _scopedSubscriptionsServiceMock
                .Setup(x => x.ConfigureScopedServiceHandlerSubscription<SubscribingService, SourceEntity, object>(
                        _event1Name
                    )
                )
                .Verifiable();

            _serviceHandlerConfigurator.HasScopedSubscriptionTo(selectionAction);
        }

        [Test]
        public void HasScopedSubscription_WithEventArgsTypeMismatch_ShouldThrow()
        {
            Assert.That(() =>
            {
                _serviceHandlerConfigurator.HasScopedSubscriptionTo(_event2Name);
            }, Throws.TypeOf<EventArgsTypeMismatchException>());
        }

        [Test]
        public void HasScopedSubscription_WithEventNotFound_ShouldThrow()
        {
            Assert.That(() =>
            {
                _serviceHandlerConfigurator.HasScopedSubscriptionTo("");
            }, Throws.TypeOf<EventFieldNotFoundException>());
        }

        private class SourceEntity
        {
#pragma warning disable 67
            public event EventHandler<object> Event;
            public event EventHandler Event2;
#pragma warning restore 67
        }

        private class SubscribingService : IEventHandler<SourceEntity, object>
        {
            public Task HandleEventAsync(SourceEntity source, object args)
            {
                throw new NotImplementedException();
            }
        }
    }
}
