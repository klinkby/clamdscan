using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Klinkby.Clam
{
    public interface IClamClient : IDisposable
    {
        Task<IReadOnlyList<string>> ExecuteCommandAsync(string command, Stream data = null);
    }
}