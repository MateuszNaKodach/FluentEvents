namespace FluentEvents.Azure.SignalR.Client
{
    internal interface IUrlProvider
    {
        string GetUrl(string endpoint, PublicationMethod publicationMethod, string hubName, string receiverId);
    }
}