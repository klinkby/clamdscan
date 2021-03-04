using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Klinkby.Clam
{
    /// <summary>
    /// https://github.com/michaelhans/Clamson
    /// </summary>
    internal static class NetworkStreamExtensions
    {
        private readonly static Encoding TextEncoding = Encoding.ASCII;
        private readonly static int BufferSize = 8192;

        internal static async Task SendTextAsync(this NetworkStream stream, string text)
        {
            using (var sw = new StreamWriter(stream, TextEncoding, text.Length, true))
            {
                await sw.WriteAsync(text);
            }
        }

        internal static async Task<string> ReceiveTextAsync(this NetworkStream stream)
        {
            using (var sr = new StreamReader(stream, TextEncoding, false, BufferSize, true))
            {
                return await sr.ReadToEndAsync();
            }
        }

        internal static async Task SendDataAsync(this NetworkStream stream, Stream data, int chunkSize)
        {
            int dataIndex = 0;
            byte[] chunkBytesLen = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(chunkSize));
            var fileBytes = new byte[chunkSize];
            while (stream.CanWrite && dataIndex < data.Length)
            {
                if (dataIndex + chunkSize >= data.Length)
                {
                    chunkSize = (int)data.Length - dataIndex;
                    chunkBytesLen = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(chunkSize));
                }
                await data.ReadAsync(fileBytes, 0, chunkSize);
                await stream.WriteAsync(chunkBytesLen, 0, chunkBytesLen.Length);
                await stream.WriteAsync(fileBytes, 0, chunkSize);
                dataIndex += chunkSize;
            }
            byte[] nullByte = BitConverter.GetBytes(0);
            await stream.WriteAsync(nullByte, 0, nullByte.Length);
        }
    }
}
