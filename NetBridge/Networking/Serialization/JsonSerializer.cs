using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace NetBridge.Networking.Serialization
{
    internal static class JsonByteArraySerializer
    {
        internal static byte[] SerializeToJsonBytes(object obj)
        {
            string jsonData = JsonSerializer.Serialize(obj);
            return System.Text.Encoding.UTF8.GetBytes(jsonData);
        }

        internal static T DeserializeFromJsonBytes<T>(byte[] data)
        {
            string jsonData = System.Text.Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
