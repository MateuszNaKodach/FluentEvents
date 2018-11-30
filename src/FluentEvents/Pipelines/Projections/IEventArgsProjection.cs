namespace FluentEvents.Pipelines.Projections
{
    public interface IEventArgsProjection
    {
        object Convert(object obj);
    }
}