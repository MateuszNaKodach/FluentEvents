using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FluentEvents.Model
{
    /// <inheritdoc />
    public class SourceModelsService : ISourceModelsService
    {
        private readonly ConcurrentDictionary<Type, SourceModel> _sourceModels;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SourceModelsService()
        {
            _sourceModels = new ConcurrentDictionary<Type, SourceModel>();
        }

        /// <inheritdoc />
        public SourceModel GetSourceModel(Type crlType)
        {
            _sourceModels.TryGetValue(crlType, out var sourceModel);
            return sourceModel;
        }

        /// <inheritdoc />
        public SourceModel GetOrCreateSourceModel(Type clrType)
        {
            return _sourceModels.GetOrAdd(clrType, new SourceModel(clrType));
        }

        /// <inheritdoc />
        public IEnumerable<SourceModel> GetSourceModels()
        {
            return _sourceModels.Values.ToList();
        }
    }
}
