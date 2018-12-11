namespace FluentEvents.Azure.SignalR
{
    internal interface IConnectionStringBuilder
    {
        ConnectionString ParseConnectionString(string connectionString);
    }
}