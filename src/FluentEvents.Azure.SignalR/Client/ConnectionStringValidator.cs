namespace FluentEvents.Azure.SignalR.Client
{
    internal static class ConnectionStringValidator
    {
        internal static string Validate(string connectionString)
        {
            ConnectionString.Parse(connectionString);

            return connectionString;
        }
    }
}