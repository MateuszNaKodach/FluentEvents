namespace FluentEvents.Infrastructure
{
    internal static class InfrastructureExtensions
    {
        internal static T Get<T>(this IInfrastructure<T> infrastructure)
            where T : class
        {
            return infrastructure.Instance;
        }
    }
}
