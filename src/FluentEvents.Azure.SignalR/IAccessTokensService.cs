using System;

namespace FluentEvents.Azure.SignalR
{
    internal interface IAccessTokensService
    {
        string GenerateAccessToken(ConnectionString connectionString, string audience, TimeSpan? lifetime = null);
    }
}