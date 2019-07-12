using System;

namespace FluentEvents.Pipelines.Projections
{
    internal class EventProjection<TFrom, TTo> : IEventProjection
    {
        private readonly Func<TFrom, TTo> _conversionFunc;

        internal EventProjection(Func<TFrom, TTo> conversionFunc)
        {
            _conversionFunc = conversionFunc;
        }

        public object Convert(object obj)
        {
            return _conversionFunc((TFrom) obj);
        }
    }
}
