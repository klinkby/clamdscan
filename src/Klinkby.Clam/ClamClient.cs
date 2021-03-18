using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class ClamClient : IClamClient
    {
        const string DefaultHost = "localhost";
        const int DefaultTcpPort = 3310;
        private static readonly Encoding TextEncoding = Encoding.ASCII;
        private readonly AsyncLazy<TcpClient> connectedClient;
        private readonly ILogger logger;

        public ILogger Logger => logger;

        public ClamClient(string host = DefaultHost, int port = DefaultTcpPort, ILogger logger = null)
        {
            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            this.logger = logger ?? NullLogger.Instance;
            connectedClient = new AsyncLazy<TcpClient>(async () =>
            {
                var newClient = new TcpClient();
                this.logger.Log(LogLevel.Information, $"Connect to {host}:{port}");
                await newClient.ConnectAsync(host, port)
                               .ConfigureAwait(false);
                return newClient;
            });
        }

        public ClamClient(IPAddress[] ipAddresses, int port = DefaultTcpPort, ILogger logger = null)
        {
            if (ipAddresses is null)
            {
                throw new ArgumentNullException(nameof(ipAddresses));
            }
            this.logger = logger ?? NullLogger.Instance;
            connectedClient = new AsyncLazy<TcpClient>(async () =>
            {
                var newClient = new TcpClient();
                this.logger.Log(LogLevel.Information, $"Connect to ip:{port}");
                await newClient.ConnectAsync(ipAddresses, port)
                               .ConfigureAwait(false);
                return newClient;
            });
        }

        public async Task<IReadOnlyList<string>> ExecuteCommandAsync(string command, Stream data = null)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            using (logger.BeginScope($"Execute {command}"))
            {

                var client = await connectedClient;
                Debug.Assert(client.Connected);
                var stream = client.GetStream();
                Debug.Assert(stream.CanWrite);
                string formattedCommand = $"z{command}\0";
                logger.Log(LogLevel.Trace, "Send command");
                await stream.SendTextAsync(formattedCommand)
                            .ConfigureAwait(false);
                if (null != data)
                {
                    Debug.Assert(data.CanRead);
                    logger.Log(LogLevel.Trace, "Send data");
                    await stream.SendDataAsync(data, client.SendBufferSize)
                                .ConfigureAwait(false);
                }
                logger.Log(LogLevel.Trace, "Flush");
                await stream.FlushAsync()
                            .ConfigureAwait(false);
                logger.Log(LogLevel.Trace, "Receive response");
                string response = await stream.ReceiveTextAsync()
                                              .ConfigureAwait(false);
                string[] formattedResponse = response.Split(new string[] { "\0", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Assert(client.Connected);
                return formattedResponse;
            }
        }

        public void Dispose()
        {
            if (connectedClient.IsValueCreated && connectedClient.Value.IsCompleted)
            {
                var client = connectedClient.Value.Result;
                if (client.Connected)
                {
                    client.Close();
                }
                client.Dispose();
            }
        }
    }
}
