using System;

namespace FluentEvents.Pipelines.Projections
{
    public class EventsSenderProjection<TFrom, TTo> : IEventsSenderProjection
    {
        private readonly Func<TFrom, TTo> m_ConversionFunc;

        public EventsSenderProjection(Func<TFrom, TTo> mConversionFunc)
        {
            m_ConversionFunc = mConversionFunc;
        }

        public object Convert(object obj)
        {
            return m_ConversionFunc((TFrom) obj);
        }
    }
}
