using System;

namespace FluentEvents
{
    public interface IInternalServiceProvider
    {
        IServiceProvider InternalServiceProvider { get; }
    }
}