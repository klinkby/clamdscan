using Klinkby.Clam;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Klinkby.ClamTests
{
    public class IntegrationTest
    {
        [Fact]
        public async Task PingAsync()
        {
            using (var client = new ClamClient())
            {
                await client.PingAsync();
            }
        }

        [Fact]
        public async Task ScanPassAsync()
        {
            using (var client = new ClamClient())
            {
                await client.InstreamAsync(new MemoryStream(new byte[] { 32 }));
            }
        }

        [Fact]
        public async Task ScanEicarAsync()
        {
            string eicar = @"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*";
            using (var client = new ClamClient())
            {
                await Assert.ThrowsAsync<ClamException>(async () => await client.InstreamAsync(new MemoryStream(Encoding.ASCII.GetBytes(eicar))));
            }
        }
    }
}
