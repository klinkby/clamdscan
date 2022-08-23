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
        public static Task PingAsync(this IClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return client.ExecuteCommandAsync("PING")
                         .ContinueWith(t => t.Result.Assert("PONG"));
        }

        public static Task<string> VersionAsync(this IClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return client.ExecuteCommandAsync("VERSION")
                         .ContinueWith(t => t.Result.ParseSingle());
        }

        public static Task<IDictionary<string, string>> StatsAsync(this IClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return client.ExecuteCommandAsync("STATS")
                         .ContinueWith(t => t.Result.ParseMultiple());
        }

        public static Task InstreamAsync(this IClamClient client, Stream data)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            return client.ExecuteCommandAsync("INSTREAM", data)
                         .ContinueWith(t => t.Result.Assert("OK"));
        }

        public static Task ReloadAsync(this IClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return client.ExecuteCommandAsync("RELOAD")
                         .ContinueWith(t => t.Result.Assert("RELOADING"));
        }

        public static Task ShutdownAsync(this IClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return client.ExecuteCommandAsync("SHUTDOWN")
                         .ContinueWith(t => t.Result.Assert(""));
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

        private static Task ScanAsync(this IClamClient client, string command, string path)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty", nameof(path));
            }
           return client.ExecuteCommandAsync($"{command} {path}")
                        .ContinueWith(t => t.Result.Assert("OK"));
        }
    }
}
