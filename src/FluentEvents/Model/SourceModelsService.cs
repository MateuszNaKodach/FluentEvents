using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FluentEvents.Model
{
    /// <inheritdoc />
    public class SourceModelsService : ISourceModelsService
    {
        private readonly ConcurrentDictionary<Type, SourceModel> m_SourceModels;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SourceModelsService()
        {
            m_SourceModels = new ConcurrentDictionary<Type, SourceModel>();
        }

        /// <inheritdoc />
        public SourceModel GetSourceModel(Type crlType)
        {
            m_SourceModels.TryGetValue(crlType, out var sourceModel);
            return sourceModel;
        }

        /// <inheritdoc />
        public SourceModel GetOrCreateSourceModel(Type clrType)
        {
            return m_SourceModels.GetOrAdd(clrType, new SourceModel(clrType));
        }

        /// <inheritdoc />
        public IEnumerable<SourceModel> GetSourceModels()
        {
            return m_SourceModels.Values.ToList();
        }
    }
}
