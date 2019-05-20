namespace FluentEvents.Pipelines.Projections
{
    internal class ProjectionPipelineModuleConfig
    {
        internal ProjectionPipelineModuleConfig(
            IEventsSenderProjection eventsSenderProjection,
            IEventArgsProjection eventArgsProjection,
            string projectedEventFieldName
        )
        {
            EventsSenderProjection = eventsSenderProjection;
            EventArgsProjection = eventArgsProjection;
            ProjectedEventFieldName = projectedEventFieldName;
        }

        internal IEventsSenderProjection EventsSenderProjection { get; }
        internal IEventArgsProjection EventArgsProjection { get; }
        internal string ProjectedEventFieldName { get; }
    }
}