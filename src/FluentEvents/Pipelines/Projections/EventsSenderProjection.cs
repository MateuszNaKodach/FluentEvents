using System;

namespace FluentEvents.Pipelines.Projections
{
    internal class EventsSenderProjection<TFrom, TTo> : IEventsSenderProjection
    {
        private readonly Func<TFrom, TTo> _conversionFunc;

        internal EventsSenderProjection(Func<TFrom, TTo> mConversionFunc)
        {
            _conversionFunc = mConversionFunc;
        }

        public object Convert(object obj)
        {
            return _conversionFunc((TFrom) obj);
        }
    }
}
