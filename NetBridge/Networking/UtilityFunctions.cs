using NetBridge.Networking.Models;
using NetBridge.Networking.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetBridge.Networking
{
    internal static class UtilityFunctions
    {
        internal static int BufferSize = 4096;

        internal static byte[] ReceiveData(NetworkStream stream)
        {
            byte[] dataSizeBuffer = new byte[4]; // Assuming 4 bytes for int (Int32)
            int bytesRead = stream.Read(dataSizeBuffer, 0, dataSizeBuffer.Length);
            int dataSize = BitConverter.ToInt32(dataSizeBuffer, 0);

            byte[] dataBuffer = new byte[dataSize];
            int totalBytesRead = 0;
            while (totalBytesRead < dataSize)
            {
                int bufferSize = Math.Min(BufferSize, dataSize - totalBytesRead);
                bytesRead = stream.Read(dataBuffer, totalBytesRead, bufferSize);
                totalBytesRead += bytesRead;
            }

            return dataBuffer;
        }

        internal static void SendData(NetworkStream stream, byte[] data)
        {
            byte[] dataSize = BitConverter.GetBytes(data.Length);
            stream.Write(dataSize, 0, dataSize.Length);

            int bytesSent = 0;
            while (bytesSent < data.Length)
            {
                int bufferSize = Math.Min(BufferSize, data.Length - bytesSent);
                stream.Write(data, bytesSent, bufferSize);
                bytesSent += bufferSize;
            }
        }

        internal static void SendObject<T>(NetworkStream stream, T data)
        {
            SendData(stream, JsonByteArraySerializer.SerializeToJsonBytes(data));
        }

        internal static T ReceiveObject<T>(NetworkStream stream)
        {
            return JsonByteArraySerializer.DeserializeFromJsonBytes<T>(UtilityFunctions.ReceiveData(stream));
        }
    }
}
