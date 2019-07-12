namespace FluentEvents.Pipelines.Projections
{
    internal interface IEventProjection
    {
        object Convert(object obj);
    }
}