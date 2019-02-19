namespace FluentEvents.Pipelines.Projections
{
    internal interface IEventsSenderProjection
    {
        object Convert(object obj);
    }
}