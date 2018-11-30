using System;

namespace FluentEvents.Pipelines.Projections
{
    public class ProjectionSelection<TFrom>
    {
        private readonly Action<Func<TFrom, object>> m_CallBack;
        private readonly Action<Type> m_ToTypeCallBack;

        internal ProjectionSelection(Action<Func<TFrom, object>> callBack, Action<Type> toTypeCallBack)
        {
            m_CallBack = callBack;
            m_ToTypeCallBack = toTypeCallBack;
        }

        public void To<TTo>(Func<TFrom, TTo> func)
            where TTo : class
        {
            m_CallBack.Invoke(func);
            m_ToTypeCallBack.Invoke(typeof(TTo));
        }
    }
}