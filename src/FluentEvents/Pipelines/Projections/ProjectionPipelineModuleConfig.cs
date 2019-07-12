namespace FluentEvents.Pipelines.Projections
{
    internal class ProjectionPipelineModuleConfig
    {
        internal IEventProjection EventProjection { get; }

        internal ProjectionPipelineModuleConfig(IEventProjection eventProjection)
        {
            EventProjection = eventProjection;
        }
    }
}