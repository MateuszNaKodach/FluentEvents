using System;
using System.Collections.Generic;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class ConnectionString
    {
        private static readonly char[] PropertySeparator = { ';' };
        private static readonly char[] KeyValueSeparator = { '=' };
        private const string EndpointProperty = "endpoint";
        private const string AccessKeyProperty = "accesskey";

        public string Endpoint { get; }
        public string AccessKey { get; }

        private ConnectionString(string endpoint, string accessKey)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            AccessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
        }

        public static implicit operator ConnectionString(string connectionString)
        {
            return Parse(connectionString);
        }

        public static implicit operator string(ConnectionString connectionString)
        {
            return connectionString.ToString();
        }

        internal static ConnectionString Parse(string connectionString)
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
                        throw new ConnectionStringHasDuplicatedPropertiesException(key);

                    dict.Add(key, kvp[1].Trim());
                }

                if (dict.ContainsKey(EndpointProperty) && dict.ContainsKey(AccessKeyProperty))
                    return new ConnectionString(dict[EndpointProperty].TrimEnd('/'), dict[AccessKeyProperty]);
            }

            throw new ConnectionStringHasMissingPropertiesException(EndpointProperty, AccessKeyProperty);
        }

        public override string ToString()
        {
            return $"{nameof(Endpoint)}{KeyValueSeparator}{Endpoint}{PropertySeparator}" +
                   $"{nameof(AccessKey)}{KeyValueSeparator}{AccessKey}{PropertySeparator}";
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when parsing a connection string with missing properties.
    /// </summary>
    public class ConnectionStringHasMissingPropertiesException : FluentEventsException
    {
        internal ConnectionStringHasMissingPropertiesException(string endpointProperty, string accessKeyProperty)
            : base($"Connection string missing required properties {endpointProperty} and {accessKeyProperty}.")
        {
            
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when parsing a connection string with duplicated properties.
    /// </summary>
    public class ConnectionStringHasDuplicatedPropertiesException : FluentEventsException
    {
        internal ConnectionStringHasDuplicatedPropertiesException(string key)
            : base($"Duplicate properties found in connection string: {key}.")
        {
            
        }
    }
}