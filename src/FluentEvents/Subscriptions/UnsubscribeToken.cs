namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     A token that can be used to cancel a global subscription with the
    ///     <see cref="EventsContext.Unsubscribe(UnsubscribeToken)"/> method.
    /// </summary>
    public sealed class UnsubscribeToken
    {
        internal Subscription Subscription { get; }

        internal UnsubscribeToken(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}