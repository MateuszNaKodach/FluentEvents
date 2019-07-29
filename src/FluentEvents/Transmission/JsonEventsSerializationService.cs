using System.Reflection;
using System.Text;
using FluentEvents.Pipelines;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FluentEvents.Transmission
{
    internal class JsonEventsSerializationService : IEventsSerializationService
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public JsonEventsSerializationService()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new CustomResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public byte[] SerializeEvent(PipelineEvent pipelineEvent)
        {
            var data = JsonConvert.SerializeObject(pipelineEvent, pipelineEvent.EventType, _serializerSettings);
            return Encoding.UTF8.GetBytes(data);
        }

        public PipelineEvent DeserializeEvent(byte[] eventData)
        {
            var stringData = Encoding.UTF8.GetString(eventData);
            return (PipelineEvent) JsonConvert.DeserializeObject(stringData, _serializerSettings);
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
