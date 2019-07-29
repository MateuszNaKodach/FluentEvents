using System;
using FluentEvents.Configuration;
using FluentEvents.Plugins;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class EventsContextOptionsTests
    {
        private EventsContextOptions _eventsContextOptions;

        [SetUp]
        public void SetUp()
        {
            _eventsContextOptions = new EventsContextOptions();
        }

        [Test]
        public void AddPlugin_WithNullPlugin_ShouldThrow()
        {
            Assert.That(() =>
            {
                ((IFluentEventsPluginOptions) _eventsContextOptions).AddPlugin(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void AddPlugin_CalledTwiceWithSamePluginType_ShouldThrow()
        {
            var plugin1 = new TestPlugin<object>();
            var plugin2 = new TestPlugin<object>();

            Assert.That(() =>
            {
                ((IFluentEventsPluginOptions)_eventsContextOptions).AddPlugin(plugin1);
                ((IFluentEventsPluginOptions)_eventsContextOptions).AddPlugin(plugin2);
            }, Throws.TypeOf<DuplicatePluginException>());
        }

        [Test]
        public void AddPlugin_ShouldAdd()
        {
            var plugin = new TestPlugin<object>();

            ((IFluentEventsPluginOptions)_eventsContextOptions).AddPlugin(plugin);

            Assert.That(((IFluentEventsPluginOptions) _eventsContextOptions).Plugins, Has.One.Items.EqualTo(plugin));
        }

        private class TestPlugin<T> : IFluentEventsPlugin
        {
            public void ApplyServices(IServiceCollection services)
            {
            }
            
        }
    }
}
