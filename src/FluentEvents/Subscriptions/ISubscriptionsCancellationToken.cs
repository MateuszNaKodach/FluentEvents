namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     A token that can be used to cancel a global subscription with the
    ///     <see cref="EventsContext.Unsubscribe(ISubscriptionsCancellationToken)"/> method.
    /// </summary>
    public interface ISubscriptionsCancellationToken
    {
    }
}