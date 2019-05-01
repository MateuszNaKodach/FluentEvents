using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentEvents.Model;
using FluentEvents.Subscriptions;
using FluentEvents.Utils;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Utils
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
        public void GetSelectedEvent_WithValidSelectionAction_ShouldAddDynamicHandlerToInvocationList(
            [Values(
                nameof(TestSource.TestEvent1),
                nameof(TestSource.TestEvent2),
                nameof(TestSource.TestEvent3),
                nameof(TestSource.TestEvent4)
            )] string eventName
        )
        {
            Action<object> subscriptionAction = null;
            _subscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(_sourceModel.ClrType, _sourceModel.ClrTypeFieldInfos, It.IsAny<Action<object>>()))
                .Callback<Type, IEnumerable<FieldInfo>, Action<object>>((_, __, action) => subscriptionAction = action)
                .Returns(new [] { new SubscribedHandler(eventName, null) })
                .Verifiable();

            _eventSelectionService.GetSelectedEvent<TestSource>(
                _sourceModel,
                (source, eventHandler) => source.TestEvent1 += (dynamic)eventHandler
            );

            var testSource = new TestSource();
            subscriptionAction(testSource);

            Assert.That(testSource.GetMergedInvocationList(), Has.Exactly(1).Items);
        }

        [Test]
        [TestCaseSource(nameof(InvalidSelectionActions))]
        public void GetSelectedEvent_WithInvalidSelectionAction_ShouldThrow(Action<TestSource, dynamic> selectionAction)
        {
            Action<object> subscriptionAction = null;
            _subscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(_sourceModel.ClrType, _sourceModel.ClrTypeFieldInfos, It.IsAny<Action<object>>()))
                .Callback<Type, IEnumerable<FieldInfo>, Action<object>>((_, __, action) => subscriptionAction = action)
                .Returns(new SubscribedHandler[0])
                .Verifiable();

            _eventSelectionService.GetSelectedEvent(
                _sourceModel,
                selectionAction
            );

            var testSource = new TestSource();

            Assert.That(() =>
            {
                subscriptionAction(testSource);
            }, Throws.TypeOf<InvalidEventSelectionException>());
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
            public event Func<Task, object, object> TestEvent4;

            public Delegate[] GetMergedInvocationList()
                => new Delegate[]
                    {
                        TestEvent1,
                        TestEvent2,
                        TestEvent3,
                        TestEvent4,
                    }
                    .Where(x => x != null)
                    .SelectMany(x => x.GetInvocationList())
                    .ToArray();
        }
    }
}
