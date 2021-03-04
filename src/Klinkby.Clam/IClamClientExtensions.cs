using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Klinkby.Clam
{
    /// <summary>
    /// https://www.clamav.net/documents/scanning#clamd
/*
Not supported are:
FILDES (e.g. STDIN)
IDSESSION, END (batching)

Deprecated are:
RAWSCAN
 */
    /// </summary>
    public static class IClamClientExtensions
    {
        public async static Task PingAsync(this IClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            (await client.ExecuteCommandAsync("PING")).Assert("PONG");
        }

        public async static Task<string> VersionAsync(this IClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return (await client.ExecuteCommandAsync("VERSION")).ParseSingle();
        }

        public async static Task<IDictionary<string, string>> StatsAsync(this IClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return (await client.ExecuteCommandAsync("STATS")).ParseMultiple();
        }

        public async static Task InstreamAsync(this IClamClient client, Stream data)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            (await client.ExecuteCommandAsync("INSTREAM", data)).Assert("OK");
        }

        public async static Task ReloadAsync(this IClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            (await client.ExecuteCommandAsync("RELOAD")).Assert("RELOADING");
        }

        public async static Task ShutdownAsync(this IClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            (await client.ExecuteCommandAsync("SHUTDOWN")).Assert("");
        }

        public static Task ScanAsync(this IClamClient client, string path)
        {
            return ScanAsync(client, "SCAN", path);
        }
       
        public static Task ContScanAsync(this IClamClient client, string path)
        {
            return ScanAsync(client, "CONTSCAN", path);
        }

        public static Task MultiScanAsync(this IClamClient client, string path)
        {
            return ScanAsync(client, "MULTISCAN", path);
        }

        public static Task AllMatchScanAsync(this IClamClient client, string path)
        {
            return ScanAsync(client, "ALLMATCHSCAN", path);
        }

        private async static Task ScanAsync(this IClamClient client, string command, string path)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty", nameof(path));
            }
           (await client.ExecuteCommandAsync($"{command} {path}")).Assert("OK");
        }
    }
}
