using System;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class UrlProvider : IUrlProvider
    {
        public string GetUrl(string endpoint, PublicationMethod publicationMethod, string hubName, string receiverId)
        {
            string url;
            switch (publicationMethod)
            {
                case PublicationMethod.User:
                    url = GetSendToUserUrl(endpoint, hubName, receiverId);
                    break;
                case PublicationMethod.Group:
                    url = GetSendToGroupUrl(endpoint, hubName, receiverId);
                    break;
                case PublicationMethod.Broadcast:
                    url = GetBroadcastUrl(endpoint, hubName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(publicationMethod));
            }

            return url;
        }

        private string GetSendToUserUrl(string endpoint, string hubName, string userId) => $"{GetBaseUrl(endpoint, hubName)}/user/{userId}";
        private string GetSendToGroupUrl(string endpoint, string hubName, string group) => $"{GetBaseUrl(endpoint, hubName)}/group/{group}";
        private string GetBroadcastUrl(string endpoint, string hubName) => $"{GetBaseUrl(endpoint, hubName)}";
        private string GetBaseUrl(string endpoint, string hubName) => $"{endpoint.TrimEnd('/')}/api/v1/hubs/{hubName.ToLower()}";
    }
}