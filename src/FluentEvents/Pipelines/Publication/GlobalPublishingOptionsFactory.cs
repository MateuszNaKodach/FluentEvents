namespace FluentEvents.Pipelines.Publication
{
    public class GlobalPublishingOptionsFactory : IGlobalPublishingOptionsFactory
    {
        GlobalPublishingOptions IGlobalPublishingOptionsFactory.With<T>() => new GlobalPublishingOptions(typeof(T));

        internal GlobalPublishingOptions Locally() => new GlobalPublishingOptions(null);
    }
}