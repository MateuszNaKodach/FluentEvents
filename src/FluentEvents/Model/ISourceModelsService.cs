using System;
using System.Collections.Generic;

namespace FluentEvents.Model
{
    /// <summary>
    ///     Provides an API surface for getting or creating <see cref="SourceModel"/>s.
    /// </summary>
    public interface ISourceModelsService
    {
        /// <summary>
        ///     Gets a previously created <see cref="SourceModel"/>.
        /// </summary>
        /// <param name="crlType">The <see cref="Type"/> of the events source.</param>
        /// <returns>The <see cref="SourceModel"/> for this <see cref="Type"/> if exists, otherwise null.</returns>
        SourceModel GetSourceModel(Type crlType);

        /// <summary>
        ///     Gets or creates a new <see cref="SourceModel"/>.
        /// </summary>
        /// <param name="clrType">The <see cref="Type"/> of the events source.</param>
        /// <returns>The <see cref="SourceModel"/> for this <see cref="Type"/>.</returns>
        SourceModel GetOrCreateSourceModel(Type clrType);

        /// <summary>
        ///     Gets all the <see cref="SourceModel"/> created with the <see cref="GetOrCreateSourceModel"/> method.
        /// </summary>
        IEnumerable<SourceModel> GetSourceModels();
    }
}