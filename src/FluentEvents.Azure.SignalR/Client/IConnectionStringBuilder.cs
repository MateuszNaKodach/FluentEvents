namespace FluentEvents.Azure.SignalR.Client
{
    internal interface IConnectionStringBuilder
    {
        ConnectionString ParseConnectionString(string connectionString);
    }
}