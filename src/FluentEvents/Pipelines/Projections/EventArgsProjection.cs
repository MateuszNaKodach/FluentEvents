using System;

namespace FluentEvents.Pipelines.Projections
{
    public class EventArgsProjection<TFrom, TTo> : IEventArgsProjection
    {
        private readonly Func<TFrom, TTo> m_ConversionFunc;

        public EventArgsProjection(Func<TFrom, TTo> conversionFunc)
        {
            m_ConversionFunc = conversionFunc;
        }

        public object Convert(object obj)
        {
            return m_ConversionFunc((TFrom) obj);
        }
    }
}
