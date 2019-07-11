namespace FluentEvents.Pipelines.Projections
{
    internal class ProjectionPipelineModuleConfig
    {
        internal IEventArgsProjection EventProjection { get; }

        internal ProjectionPipelineModuleConfig(IEventArgsProjection eventProjection)
        {
            EventProjection = eventProjection;
        }
    }
}