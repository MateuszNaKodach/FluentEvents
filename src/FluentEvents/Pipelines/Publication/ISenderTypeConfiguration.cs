using System;

namespace FluentEvents.Pipelines.Publication
{
    public interface ISenderTypeConfiguration
    {
        Type SenderType { get; }
    }
}