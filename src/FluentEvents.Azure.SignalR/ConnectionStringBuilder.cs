using System;
using System.Collections.Generic;
using System.Text;

namespace FluentEvents.Azure.SignalR
{
    internal class ConnectionStringBuilder : IConnectionStringBuilder
    {
        private static readonly char[] PropertySeparator = { ';' };
        private static readonly char[] KeyValueSeparator = { '=' };
        private const string EndpointProperty = "endpoint";
        private const string AccessKeyProperty = "accesskey";

        public ConnectionString ParseConnectionString(string connectionString)
        {
            var properties = connectionString.Split(PropertySeparator, StringSplitOptions.RemoveEmptyEntries);
            if (properties.Length > 1)
            {
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var property in properties)
                {
                    var kvp = property.Split(KeyValueSeparator, 2);
                    if (kvp.Length != 2) continue;

                    var key = kvp[0].Trim();
                    if (dict.ContainsKey(key))
                    {
                        throw new ArgumentException($"Duplicate properties found in connection string: {key}.");
                    }

                    dict.Add(key, kvp[1].Trim());
                }

                if (dict.ContainsKey(EndpointProperty) && dict.ContainsKey(AccessKeyProperty))
                    return new ConnectionString(dict[EndpointProperty].TrimEnd('/'), dict[AccessKeyProperty]);
            }

            throw new ArgumentException($"Connection string missing required properties {EndpointProperty} and {AccessKeyProperty}.");
        }
    }
}
