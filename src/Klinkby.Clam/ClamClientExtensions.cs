using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Klinkby.Clam
{
    /// <summary>
    /// https://www.clamav.net/documents/scanning#clamd
    /// </summary>
    public static class ClamClientExtensions
    {
        private static void AssertResponse(IReadOnlyList<string> response, string expected)
        {
            var singleResponse = response.SingleOrDefault();
            if (expected != singleResponse)
            {
                throw new ClamException(singleResponse);
            }
        }

        public async static Task InstreamAsync(this ClamClient client, Stream data)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var res = await client.ExecuteCommandAsync("INSTREAM", data);
            AssertResponse(res, "stream: OK");
        }

        public async static Task PingAsync(this ClamClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            var res = await client.ExecuteCommandAsync("PING");
            AssertResponse(res, "PONG");
        }

    }
}
