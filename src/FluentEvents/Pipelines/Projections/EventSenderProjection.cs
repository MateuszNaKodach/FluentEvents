using System;

namespace FluentEvents.Pipelines.Projections
{
    internal class EventSenderProjection<TFrom, TTo> : IEventsSenderProjection
    {
        private readonly Func<TFrom, TTo> _conversionFunc;

        internal EventSenderProjection(Func<TFrom, TTo> conversionFunc)
        {
            _conversionFunc = conversionFunc;
        }

        public object Convert(object obj)
        {
            return _conversionFunc((TFrom) obj);
        }
    }
}
