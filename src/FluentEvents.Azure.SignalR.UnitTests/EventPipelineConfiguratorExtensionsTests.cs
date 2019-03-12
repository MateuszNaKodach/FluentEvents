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

        private Mock<IPipeline> m_PipelineMock;
        private SourceModel m_SourceModel;
        private SourceModelEventField m_SourceModelEventField;
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private EventPipelineConfigurator<TestEntity, TestEventArgs> m_EventPipelineConfigurator;

        [SetUp]
        public void SetUp()
        {
            m_SourceModel = new SourceModel(typeof(TestEntity));
            m_SourceModelEventField = m_SourceModel.GetOrCreateEventField(nameof(TestEntity.TestEvent));
            m_PipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            
            m_EventPipelineConfigurator = new EventPipelineConfigurator<TestEntity, TestEventArgs>(
                m_SourceModel,
                m_SourceModelEventField,
                m_ServiceProviderMock.Object,
                m_PipelineMock.Object
            );
        }

        [Test]
        public void ThenIsSentToAllAzureSignalRUsers_ShouldAddAzureSignalRPipelineModule(
            [Values] bool isHubNameNull,
            [Values] bool isHubMethodNameNull
        )
        {
            AzureSignalRPipelineModuleConfig config = null;
            m_PipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            var hubName = isHubNameNull ? null : HubName;
            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            m_EventPipelineConfigurator.ThenIsSentToAllAzureSignalRUsers(hubName, hubMethodName);

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
            m_PipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            string[] UserIdsProviderAction(TestEntity source, TestEventArgs args) => new[] {""};

            var hubName = isHubNameNull ? null : HubName;
            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            m_EventPipelineConfigurator.ThenIsSentToAzureSignalRUsers(UserIdsProviderAction, hubName, hubMethodName);

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
            m_PipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            string[] GroupIdsProviderAction(TestEntity source, TestEventArgs args) => new[] { "" };

            var hubName = isHubNameNull ? null : HubName;
            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            m_EventPipelineConfigurator.ThenIsSentToAzureSignalRGroups(GroupIdsProviderAction, hubName, hubMethodName);

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
