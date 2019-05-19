namespace WorkerSample.Notifications
{
    public interface IMailService
    {
        void SendSubscriptionCancelledEmail(string emailAddress);
    }
}
