using System;
using FluentEvents.Configuration;
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
        private Mock<IServiceProvider> _serviceProviderMock;

        private EventPipelineConfigurator<TestEvent> _eventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            
            _eventPipelineConfigurator = new EventPipelineConfigurator<TestEvent>(
                _serviceProviderMock.Object,
                _pipelineMock.Object
            );
        }

        [Test]
        public void ThenIsSentToAllAzureSignalRUsers_ShouldAddAzureSignalRPipelineModule(
            [Values] bool isHubMethodNameNull
        )
        {
            AzureSignalRPipelineModuleConfig config = null;
            _pipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            _eventPipelineConfigurator.ThenIsSentToAllAzureSignalRUsers(HubName, hubMethodName);

            AssertThatHubNameAndHubMethodNamesAreCorrect(isHubMethodNameNull, config);
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
            [Values] bool isHubMethodNameNull
        )
        {
            AzureSignalRPipelineModuleConfig config = null;
            _pipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            string[] UserIdsProviderAction(TestEvent args) => new[] {""};

            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            _eventPipelineConfigurator.ThenIsSentToAzureSignalRUsers(UserIdsProviderAction, HubName, hubMethodName);

            AssertThatHubNameAndHubMethodNamesAreCorrect(isHubMethodNameNull, config);
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
            [Values] bool isHubMethodNameNull
        )
        {
            AzureSignalRPipelineModuleConfig config = null;
            _pipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            string[] GroupIdsProviderAction(TestEvent args) => new[] { "" };

            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            _eventPipelineConfigurator.ThenIsSentToAzureSignalRGroups(GroupIdsProviderAction, HubName, hubMethodName);

            AssertThatHubNameAndHubMethodNamesAreCorrect(isHubMethodNameNull, config);
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.PublicationMethod)).EqualTo(PublicationMethod.Group)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.ReceiverIdsProviderAction)).Not.Null
            );
        }

        private static void AssertThatHubNameAndHubMethodNamesAreCorrect(bool isHubMethodNameNull,
            AzureSignalRPipelineModuleConfig config)
        {
            var expectedHubMethodName = isHubMethodNameNull ? nameof(TestEvent) : HubMethodName;

            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.HubMethodName)).EqualTo(expectedHubMethodName)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.HubName)).EqualTo(HubName)
            );
        }
        
        private class TestEvent
        {
        }
    }
}
