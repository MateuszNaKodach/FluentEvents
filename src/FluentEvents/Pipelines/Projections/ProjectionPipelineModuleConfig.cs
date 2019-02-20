namespace FluentEvents.Pipelines.Projections
{
    internal class ProjectionPipelineModuleConfig
    {
        internal ProjectionPipelineModuleConfig(
            IEventsSenderProjection eventsSenderProjection,
            IEventArgsProjection eventArgsProjection
        )
        {
            EventsSenderProjection = eventsSenderProjection;
            EventArgsProjection = eventArgsProjection;
        }

        internal IEventsSenderProjection EventsSenderProjection { get; }
        internal IEventArgsProjection EventArgsProjection { get; }
    }
}