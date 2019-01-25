using System.Reflection;
using FluentEvents.Pipelines;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FluentEvents.Transmission
{
    public class JsonEventsSerializationService : IEventsSerializationService
    {
        private readonly JsonSerializerSettings m_SerializerSettings;

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

        public string SerializeEvent(PipelineEvent pipelineEvent)
        {
            var data = JsonConvert.SerializeObject(pipelineEvent, m_SerializerSettings);
            return data;
        }

        public PipelineEvent DeserializeEvent(string jsonEventData)
        {
            return (PipelineEvent) JsonConvert.DeserializeObject(jsonEventData, m_SerializerSettings);
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
