using System.Reflection;
using System.Text;
using FluentEvents.Pipelines;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FluentEvents.Transmission
{
    /// <inheritdoc />
    public class JsonEventsSerializationService : IEventsSerializationService
    {
        private readonly JsonSerializerSettings m_SerializerSettings;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public JsonEventsSerializationService()
        {
            m_SerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new CustomResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        /// <inheritdoc />
        public byte[] SerializeEvent(PipelineEvent pipelineEvent)
        {
            var data = JsonConvert.SerializeObject(pipelineEvent, pipelineEvent.OriginalSenderType, m_SerializerSettings);
            return Encoding.UTF8.GetBytes(data);
        }

        /// <inheritdoc />
        public PipelineEvent DeserializeEvent(byte[] eventData)
        {
            var stringData = Encoding.UTF8.GetString(eventData);
            return (PipelineEvent) JsonConvert.DeserializeObject(stringData, m_SerializerSettings);
        }

        private class CustomResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);
                
                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }
    }
}
