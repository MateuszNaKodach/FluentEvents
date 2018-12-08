namespace FluentEvents.Infrastructure
{
    public interface IInfrastructure<out T> where T : class
    {
        T Instance { get; }
    }
}
