using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetBridge.Networking.Serialization
{
    internal static class JsonByteArraySerializer
    {
        internal static byte[] SerializeToJsonBytes<T>(T source)
        {
            string jsonData = JsonSerializer.Serialize<T>(source);
            return System.Text.Encoding.UTF8.GetBytes(jsonData);
        }

        internal static T DeserializeFromJsonBytes<T>(byte[] data)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                IncludeFields = true,
                IgnoreNullValues = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                },
            };
            string jsonData = System.Text.Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(jsonData, options);
        }
    }
}
