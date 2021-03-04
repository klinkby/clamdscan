using Klinkby.Clam;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Klinkby.ClamTests
{
    public class IntegrationTest
    {
        readonly Func<ClamClient> factory;
        const string testPath = "/bin";

        public IntegrationTest(ITestOutputHelper output)
        {
            factory = () => new ClamClient(logger: new LoggerAdapter(output));
        }

        [Fact]
        public async Task PingAsync()
        {
            using (var client = this.factory())
            {
                await client.PingAsync();
            }
        }

        [Fact]
        public async Task ReloadAsync()
        {
            using (var client = this.factory())
            {
                await client.ReloadAsync();
            }
        }

        [Fact]
        public async Task ShutdownAsync()
        {
            using (var client = this.factory())
            {
        //        await client.ShutdownAsync();
            }
        }

        [Fact]
        public async Task ScanPassAsync()
        {
            using (var client = this.factory())
            {
                await client.InstreamAsync(new MemoryStream(new byte[] { 32 }));
            }
        }

        [Fact]
        public async Task ScanEicarAsync()
        {
            string eicar = @"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*";
            using (var client = this.factory())
            {
                await Assert.ThrowsAsync<ClamException>(async () => await client.InstreamAsync(new MemoryStream(Encoding.ASCII.GetBytes(eicar))));
            }
        }

        [Fact]
        public async Task ReuseAsync()
        {
            using (var client = this.factory())
            {
                await client.PingAsync();
                await client.InstreamAsync(new MemoryStream(new byte[] { 32 }));
                await client.PingAsync();
                await client.InstreamAsync(new MemoryStream(new byte[] { 32 }));
            }
        }

        [Fact]
        public async Task MultipleClients()
        {
            using (var client = this.factory())
            {
            }
            using (var client = this.factory())
            {
            }
            using (var client = this.factory())
            {
                await client.PingAsync();
                using (var client2 = this.factory())
                {
                    await client.PingAsync();
                }
            }
        }

        [Fact]
        public async Task VersionAsync()
        {
            using (var client = this.factory())
            {
                string ver = await client.VersionAsync();
                Assert.NotEmpty(ver);
                client.Logger.LogInformation(ver);
            }
        }

        [Fact]
        public async Task ScanAsync()
        {
            using (var client = this.factory())
            {
                await client.ScanAsync(testPath);
            }
        }

        [Fact]
        public async Task MultiScanAsync()
        {
            using (var client = this.factory())
            {
                await client.MultiScanAsync(testPath);
            }
        }

        [Fact]
        public async Task ContScanAsync()
        {
            using (var client = this.factory())
            {
                await client.ContScanAsync(testPath);
            }
        }

        [Fact]
        public async Task AllMatchScanAsync()
        {
            using (var client = this.factory())
            {
                await client.AllMatchScanAsync(testPath);
            }
        }

        [Fact]
        public async Task StatsAsync()
        {
            using (var client = this.factory())
            {
                var stats = await client.StatsAsync();
                Assert.NotEmpty(stats);
                foreach (var k in stats.Keys)
                {
                    client.Logger.LogInformation($"{k}\t{stats[k]}");
                }
            }
        }
    }
}
