using System;

namespace FluentEvents.Azure.SignalR.Client
{
    internal interface IAccessTokensService
    {
        string GenerateAccessToken(
            ConnectionString connectionString,
            string audience,
            string nameIdentifier = null,
            TimeSpan? lifetime = null
        );
    }
}