namespace FluentEvents.IntegrationTests.Common
{
    public class TestEvent : TestEventBase, ITestEvent
    {
        public string Value { get; set; } = "Test";
    }
}