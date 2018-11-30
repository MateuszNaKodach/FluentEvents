namespace FluentEvents.Pipelines.Projections
{
    public interface IEventsSenderProjection
    {
        object Convert(object obj);
    }
}