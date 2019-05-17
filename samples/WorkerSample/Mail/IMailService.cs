namespace WorkerSample.Mail
{
    internal interface IMailService
    {
        void SendSubscriptionCancelledEmail(string emailAddress);
    }
}
