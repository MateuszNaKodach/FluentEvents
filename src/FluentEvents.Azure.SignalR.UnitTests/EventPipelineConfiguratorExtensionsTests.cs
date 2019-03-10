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
        public void ThenIsPublishedToAllAzureSignalRUsers_ShouldAddAzureSignalRPipelineModule(
            [Values] bool isHubMethodNameNull
        )
        {
            AzureSignalRPipelineModuleConfig config = null;
            m_PipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            m_EventPipelineConfigurator.ThenIsPublishedToAllAzureSignalRUsers(HubName, hubMethodName);

            var expectedHubMethodName = isHubMethodNameNull ? nameof(TestEntity.TestEvent) : HubMethodName;
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.HubMethodName)).EqualTo(expectedHubMethodName)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.HubName)).EqualTo(HubName)
            );
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
        public void ThenIsPublishedToAzureSignalRUsers_ShouldAddAzureSignalRPipelineModule(
            [Values] bool isHubMethodNameNull
        )
        {
            AzureSignalRPipelineModuleConfig config = null;
            m_PipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            string[] UserIdsProviderAction(TestEntity source, TestEventArgs args) => new[] {""};

            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            m_EventPipelineConfigurator.ThenIsPublishedToAzureSignalRUsers(UserIdsProviderAction, HubName, hubMethodName);

            var expectedHubMethodName = isHubMethodNameNull ? nameof(TestEntity.TestEvent) : HubMethodName;
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.HubMethodName)).EqualTo(expectedHubMethodName)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.HubName)).EqualTo(HubName)
            );
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
        public void ThenIsPublishedToAzureSignalRGroups_ShouldAddAzureSignalRPipelineModule(
            [Values] bool isHubMethodNameNull
        )
        {
            AzureSignalRPipelineModuleConfig config = null;
            m_PipelineMock
                .Setup(x => x.AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(It.IsAny<AzureSignalRPipelineModuleConfig>()))
                .Callback<AzureSignalRPipelineModuleConfig>(x => config = x)
                .Verifiable();

            string[] GroupIdsProviderAction(TestEntity source, TestEventArgs args) => new[] { "" };

            var hubMethodName = isHubMethodNameNull ? null : HubMethodName;
            m_EventPipelineConfigurator.ThenIsPublishedToAzureSignalRGroups(GroupIdsProviderAction, HubName, hubMethodName);

            var expectedHubMethodName = isHubMethodNameNull ? nameof(TestEntity.TestEvent) : HubMethodName;
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.HubMethodName)).EqualTo(expectedHubMethodName)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.HubName)).EqualTo(HubName)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.PublicationMethod)).EqualTo(PublicationMethod.Group)
            );
            Assert.That(
                config,
                Has.Property(nameof(AzureSignalRPipelineModuleConfig.ReceiverIdsProviderAction)).Not.Null
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
