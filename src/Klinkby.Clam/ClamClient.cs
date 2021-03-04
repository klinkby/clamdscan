using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Klinkby.Clam
{
    /// <summary>
    /// Non-blocking TCP client for ClamAV daemon
    /// </summary>
    public class ClamClient : IDisposable
    {
        const string DefaultHost = "localhost";
        const int DefaultTcpPort = 3310;
        readonly static Encoding TextEncoding = Encoding.ASCII;
        private readonly AsyncLazy<TcpClient> connectedClient;
        
        public ClamClient(string host = DefaultHost, int port = DefaultTcpPort)
        {
            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            connectedClient = new AsyncLazy<TcpClient>(async () => {
                var newClient = new TcpClient();                
                await newClient.ConnectAsync(host, port);
                return newClient;
            });
        }

        public ClamClient(IPAddress[] ipAddresses, int port = DefaultTcpPort)
        {
            if (ipAddresses is null)
            {
                throw new ArgumentNullException(nameof(ipAddresses));
            }
            connectedClient = new AsyncLazy<TcpClient>(async () => { 
                var newClient = new TcpClient();
                await newClient.ConnectAsync(ipAddresses, port);
                return newClient;
            });
        }

        public async Task<IReadOnlyList<string>> ExecuteCommandAsync(string command, Stream data = null)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var client = await connectedClient;
            using (var stream = client.GetStream())
            {
                string formattedCommand = $"z{command}\0";
                await stream.SendTextAsync(formattedCommand);
                if (null != data)
                {
                    await stream.SendDataAsync(data, client.SendBufferSize);
                }
                await stream.FlushAsync();
                string response = await stream.ReceiveTextAsync();
                string[] formattedResponse = response.Split(new string[] { "\0", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                stream.Close();
                return formattedResponse;
            }
        }

        public void Dispose()
        {
            if (connectedClient.IsValueCreated)
            {
                connectedClient.Value.Dispose();
            }
        }
    }
}
