using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FluentEvents.Model
{
    internal class SourceModelsService : ISourceModelsService
    {
        private readonly ConcurrentDictionary<Type, SourceModel> _sourceModels;

        public SourceModelsService()
        {
            _sourceModels = new ConcurrentDictionary<Type, SourceModel>();
        }

        public SourceModel GetOrCreateSourceModel(Type clrType)
        {
            return _sourceModels.GetOrAdd(clrType, new SourceModel(clrType));
        }
    }
}
