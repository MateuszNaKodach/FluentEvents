using System;
using System.Collections.Generic;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class ConnectionString
    {
        private static readonly char[] _propertySeparator = { ';' };
        private static readonly char[] _keyValueSeparator = { '=' };
        private const string EndpointProperty = "endpoint";
        private const string AccessKeyProperty = "accesskey";

        public string Endpoint { get; }
        public string AccessKey { get; }

        internal ConnectionString(string endpoint, string accessKey)
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

        private static ConnectionString Parse(string connectionString)
        {
            if (connectionString == null)
                throw new ConnectionStringIsNullException();

            var properties = connectionString.Split(_propertySeparator, StringSplitOptions.RemoveEmptyEntries);
            if (properties.Length > 1)
            {
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var property in properties)
                {
                    var kvp = property.Split(_keyValueSeparator, 2);
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

        public static string Validate(string connectionString)
        {
            Parse(connectionString);

            return connectionString;
        }

        public override string ToString()
        {
            return $"{nameof(Endpoint)}{new string(_keyValueSeparator)}{Endpoint}{new string(_propertySeparator)}" +
                   $"{nameof(AccessKey)}{new string(_keyValueSeparator)}{AccessKey}{new string(_propertySeparator)}";
        }
    }
}