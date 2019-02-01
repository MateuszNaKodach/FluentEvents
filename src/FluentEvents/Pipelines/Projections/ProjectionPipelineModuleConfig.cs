namespace FluentEvents.Pipelines.Projections
{
    public class ProjectionPipelineModuleConfig
    {
        public ProjectionPipelineModuleConfig(IEventsSenderProjection eventsSenderProjection, IEventArgsProjection eventArgsProjection)
        {
            EventsSenderProjection = eventsSenderProjection;
            EventArgsProjection = eventArgsProjection;
        }

        public IEventsSenderProjection EventsSenderProjection { get; }
        public IEventArgsProjection EventArgsProjection { get; }
    }
}