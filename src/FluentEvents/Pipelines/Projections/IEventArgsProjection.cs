namespace FluentEvents.Pipelines.Projections
{
    internal interface IEventArgsProjection
    {
        object Convert(object obj);
    }
}