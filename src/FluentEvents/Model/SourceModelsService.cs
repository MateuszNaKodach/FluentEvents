using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FluentEvents.Model
{
    public class SourceModelsService : ISourceModelsService
    {
        private readonly ConcurrentDictionary<Type, SourceModel> m_SourceModels;

        public SourceModelsService()
        {
            m_SourceModels = new ConcurrentDictionary<Type, SourceModel>();
        }

        public SourceModel GetSourceModel(Type crlType)
        {
            m_SourceModels.TryGetValue(crlType, out var sourceModel);
            return sourceModel;
        }

        public SourceModel GetOrCreateSourceModel(Type clrType, IEventsContext eventsContext)
        {
            return m_SourceModels.GetOrAdd(clrType, new SourceModel(clrType, eventsContext));
        }
        
        public IEnumerable<SourceModel> GetSourceModels()
        {
            return m_SourceModels.Values.ToList();
        }
    }
}
