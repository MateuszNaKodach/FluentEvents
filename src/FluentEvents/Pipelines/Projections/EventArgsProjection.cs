using System;

namespace FluentEvents.Pipelines.Projections
{
    internal class EventArgsProjection<TFrom, TTo> : IEventArgsProjection
    {
        private readonly Func<TFrom, TTo> _conversionFunc;

        internal EventArgsProjection(Func<TFrom, TTo> conversionFunc)
        {
            _conversionFunc = conversionFunc;
        }

        public object Convert(object obj)
        {
            return _conversionFunc((TFrom) obj);
        }
    }
}
