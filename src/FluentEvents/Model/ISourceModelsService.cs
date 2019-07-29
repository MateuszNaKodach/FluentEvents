using System;

namespace FluentEvents.Model
{
    /// <summary>
    ///     Provides an API surface for getting or creating <see cref="SourceModel"/>s.
    /// </summary>
    public interface ISourceModelsService
    {
        /// <summary>
        ///     Gets or creates a new <see cref="SourceModel"/>.
        /// </summary>
        /// <param name="clrType">The <see cref="Type"/> of the events source.</param>
        /// <returns>The <see cref="SourceModel"/> for this <see cref="Type"/>.</returns>
        SourceModel GetOrCreateSourceModel(Type clrType);
    }
}