using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AsyncEvent;
using FluentEvents.Config;
using FluentEvents.Model;
using FluentEvents.Subscriptions;
using FluentEvents.Utils;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class EventSelectionServiceTests
    {
        private SourceModel _sourceModel;
        private Mock<ISubscriptionScanService> _subscriptionScanServiceMock;
        private EventSelectionService _eventSelectionService;

        [SetUp]
        public void SetUp()
        {
            _sourceModel = new SourceModel(typeof(TestSource));
            _subscriptionScanServiceMock = new Mock<ISubscriptionScanService>(MockBehavior.Strict);
            _eventSelectionService = new EventSelectionService(_subscriptionScanServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionScanServiceMock.Verify();
        }

        [Test]
        public void GetSelectedEvents_WithValidSelectionAction_ShouldAddDynamicHandlerToInvocationList(
            [Values(
                nameof(TestSource.TestEvent1),
                nameof(TestSource.TestEvent2),
                nameof(TestSource.TestEvent3),
                nameof(TestSource.TestEvent4),
                nameof(TestSource.TestEvent5)
            )] string eventName
        )
        {
            Action<object> subscriptionAction = null;
            _subscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(_sourceModel.ClrType, _sourceModel.ClrTypeEventFieldInfos, It.IsAny<Action<object>>()))
                .Callback<Type, IEnumerable<FieldInfo>, Action<object>>((_, __, action) => subscriptionAction = action)
                .Returns(new [] { new SubscribedHandler(eventName, null) })
                .Verifiable();

            Action<TestSource, dynamic> subscriptionActionWithDynamic;
            switch (eventName)
            {
                case nameof(TestSource.TestEvent1):
                    subscriptionActionWithDynamic = (source, eventHandler) => source.TestEvent1 += eventHandler;
                    break;
                case nameof(TestSource.TestEvent2): 
                    subscriptionActionWithDynamic = (source, eventHandler) => source.TestEvent2 += eventHandler;
                    break;
                case nameof(TestSource.TestEvent3): 
                    subscriptionActionWithDynamic = (source, eventHandler) => source.TestEvent3 += eventHandler;
                    break;
                case nameof(TestSource.TestEvent4): 
                    subscriptionActionWithDynamic = (source, eventHandler) => source.TestEvent4 += eventHandler;
                    break;
                case nameof(TestSource.TestEvent5):
                    subscriptionActionWithDynamic = (source, eventHandler) => source.TestEvent5 += eventHandler;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventName));
            }
            _eventSelectionService.GetSelectedEventNames(
                _sourceModel,
                subscriptionActionWithDynamic
            );

            var testSource = new TestSource();
            subscriptionAction(testSource);

            Assert.That(testSource.GetMergedInvocationList(), Has.Exactly(1).Items);
        }

        [Test]
        [TestCaseSource(nameof(InvalidSelectionActions))]
        public void GetSelectedEvents_WithInvalidSelectionAction_ShouldThrow(Action<TestSource, dynamic> selectionAction)
        {
            Action<object> subscriptionAction = null;
            _subscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(_sourceModel.ClrType, _sourceModel.ClrTypeEventFieldInfos, It.IsAny<Action<object>>()))
                .Callback<Type, IEnumerable<FieldInfo>, Action<object>>((_, __, action) => subscriptionAction = action)
                .Returns(new SubscribedHandler[0])
                .Verifiable();

            _eventSelectionService.GetSelectedEventNames(
                _sourceModel,
                selectionAction
            );

            var testSource = new TestSource();

            Assert.That(() =>
            {
                subscriptionAction(testSource);
            }, Throws.TypeOf<InvalidEventSelectionException>());
        }

        [Test]
        public void GetSelectedEvents_WithInvalidEventHandlerReturnType_ShouldThrow()
        {
            Action<object> subscriptionAction = null;
            _subscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(_sourceModel.ClrType, _sourceModel.ClrTypeEventFieldInfos, It.IsAny<Action<object>>()))
                .Callback<Type, IEnumerable<FieldInfo>, Action<object>>((_, __, action) => subscriptionAction = action)
                .Returns(new SubscribedHandler[0])
                .Verifiable();

            _eventSelectionService.GetSelectedEventNames<TestSource>(
                _sourceModel,
                (source, h) => source.InvalidEventReturnType += (dynamic)h
            );

            var testSource = new TestSource();

            Assert.That(() =>
            {
                subscriptionAction(testSource);
            }, Throws.TypeOf<SelectedEventReturnTypeNotSupportedException>());
        }

        [Test]
        public void GetSingleSelectedEventName_WithOneEventSelected_ShouldReturnEventName()
        {
            _subscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(_sourceModel.ClrType, _sourceModel.ClrTypeEventFieldInfos, It.IsAny<Action<object>>()))
                .Returns(new[]
                {
                    new SubscribedHandler(nameof(TestSource.TestEvent1), null),
                })
                .Verifiable();

            void SubscriptionActionWithDynamic(TestSource source, dynamic eventHandler)
            {
            }

            _eventSelectionService.GetSingleSelectedEventName(
                _sourceModel,
                (Action<TestSource, dynamic>)SubscriptionActionWithDynamic
            );
        }

        [Test]
        public void GetSingleSelectedEventName_WithMultipleEventsSelected_ShouldThrow()
        {
            _subscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(_sourceModel.ClrType, _sourceModel.ClrTypeEventFieldInfos, It.IsAny<Action<object>>()))
                .Returns(new[]
                {
                    new SubscribedHandler(nameof(TestSource.TestEvent1), null),
                    new SubscribedHandler(nameof(TestSource.TestEvent2), null),
                })
                .Verifiable();

            void SubscriptionActionWithDynamic(TestSource source, dynamic eventHandler)
            {
            }

            Assert.That(() =>
            {
                _eventSelectionService.GetSingleSelectedEventName(
                    _sourceModel,
                    (Action<TestSource, dynamic>) SubscriptionActionWithDynamic
                );
            }, Throws.TypeOf<MoreThanOneEventSelectedException>());
        }

        [Test]
        public void GetSingleSelectedEventName_WithNoEventsSelected_ShouldThrow()
        {
            _subscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(_sourceModel.ClrType, _sourceModel.ClrTypeEventFieldInfos, It.IsAny<Action<object>>()))
                .Returns(new SubscribedHandler[0])
                .Verifiable();

            void SubscriptionActionWithDynamic(TestSource source, dynamic eventHandler)
            {
            }

            Assert.That(() =>
            {
                _eventSelectionService.GetSingleSelectedEventName(
                    _sourceModel,
                    (Action<TestSource, dynamic>)SubscriptionActionWithDynamic
                );
            }, Throws.TypeOf<NoEventsSelectedException>());
        }


        private static TestCaseData[] InvalidSelectionActions => new[]
        {
            new TestCaseData(new Action<TestSource, dynamic>((source, eventHandler) => eventHandler.Invalid())),
            new TestCaseData(new Action<TestSource, dynamic>((source, eventHandler) => ((TestSource)eventHandler).Equals(0)))
        };

        public class TestSource
        {
            public event EventHandler TestEvent1;
            public event EventHandler<object> TestEvent2;
            public event Action<object, object> TestEvent3;
            public event Func<object, object, Task> TestEvent4;
            public event AsyncEventHandler<object> TestEvent5;

            public event Func<object> InvalidEventReturnType;

            public Delegate[] GetMergedInvocationList()
                => new Delegate[]
                    {
                        TestEvent1,
                        TestEvent2,
                        TestEvent3,
                        TestEvent4,
                        TestEvent5,
                    }
                    .Where(x => x != null)
                    .SelectMany(x => x.GetInvocationList())
                    .ToArray();
        }
    }
}
