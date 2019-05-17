using System;
using FluentEvents.Config;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests
{
    [TestFixture]
    public class EventPipelineConfiguratorExtensionsTests
    {
        private const string HubName = nameof(HubName);
        private const string HubMethodName = nameof(HubMethodName);

        private Mock<IPipeline> _pipelineMock;
        private SourceModel _sourceModel;
        private SourceModelEventField _sourceModelEventField;
        private Mock<IServiceProvider> _serviceProviderMock;
        private EventPipelineConfigurator<TestEntity, TestEventArgs> _eventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            _sourceModel = new SourceModel(typeof(TestEntity));
            _sourceModelEventField = _sourceModel.GetOrCreateEventField(nameof(TestEntity.TestEvent));
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            
            _eventPipelineConfigurator = new EventPipelineConfigurator<TestEntity, TestEventArgs>(
                _sourceModel,
                _sourceModelEventField,
                _serviceProviderMock.Object,
                _pipelineMock.Object
            );
        }

        [Test]
        public void ThenIsSentToAllAzureSignalRUsers_ShouldAddAzureSignalRPipelineModule(
            [Values] bool isHubNameNull,
            [Values] bool isHubMethodNameNull
        )
        {
            AzureSignalRPipelineModuleConfig config = null;
            _pipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            var hubName = isHubNameNull ? null : HubName;
            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            _eventPipelineConfigurator.ThenIsSentToAllAzureSignalRUsers(hubName, hubMethodName);

            AssertThatHubNameAndHubMethodNamesAreCorrect(isHubNameNull, isHubMethodNameNull, config);
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.PublicationMethod)).EqualTo(PublicationMethod.Broadcast)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.ReceiverIdsProviderAction)).Null
            );
        }

        [Test]
        public void ThenIsSentToAzureSignalRUsers_ShouldAddAzureSignalRPipelineModule(
            [Values] bool isHubNameNull,
            [Values] bool isHubMethodNameNull
        )
        {
            AzureSignalRPipelineModuleConfig config = null;
            _pipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            string[] UserIdsProviderAction(TestEntity source, TestEventArgs args) => new[] {""};

            var hubName = isHubNameNull ? null : HubName;
            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            _eventPipelineConfigurator.ThenIsSentToAzureSignalRUsers(UserIdsProviderAction, hubName, hubMethodName);

            AssertThatHubNameAndHubMethodNamesAreCorrect(isHubNameNull, isHubMethodNameNull, config);
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.PublicationMethod)).EqualTo(PublicationMethod.User)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.ReceiverIdsProviderAction)).Not.Null
            );
        }

        [Test]
        public void ThenIsSentToAzureSignalRGroups_ShouldAddAzureSignalRPipelineModule(
            [Values] bool isHubNameNull,
            [Values] bool isHubMethodNameNull
        )
        {
            AzureSignalRPipelineModuleConfig config = null;
            _pipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            string[] GroupIdsProviderAction(TestEntity source, TestEventArgs args) => new[] { "" };

            var hubName = isHubNameNull ? null : HubName;
            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            _eventPipelineConfigurator.ThenIsSentToAzureSignalRGroups(GroupIdsProviderAction, hubName, hubMethodName);

            AssertThatHubNameAndHubMethodNamesAreCorrect(isHubNameNull, isHubMethodNameNull, config);
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.PublicationMethod)).EqualTo(PublicationMethod.Group)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.ReceiverIdsProviderAction)).Not.Null
            );
        }

        private static void AssertThatHubNameAndHubMethodNamesAreCorrect(bool isHubNameNull, bool isHubMethodNameNull,
            AzureSignalRPipelineModuleConfig config)
        {
            var expectedHubName = isHubNameNull ? nameof(TestEntity) + "Hub" : HubName;
            var expectedHubMethodName = isHubMethodNameNull ? nameof(TestEntity.TestEvent) : HubMethodName;

            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.HubMethodName)).EqualTo(expectedHubMethodName)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.HubName)).EqualTo(expectedHubName)
            );
        }

        private class TestEntity
        {
            public event EventHandler<TestEventArgs> TestEvent;
        }

        private class TestEventArgs
        {
        }
    }
}
