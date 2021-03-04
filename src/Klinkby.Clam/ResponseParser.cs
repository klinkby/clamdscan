using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Klinkby.Clam
{
    internal static class ResponseParser
    {
        private static readonly Regex responsePattern = new Regex(@"^(?<name>[^:]+):\ (?<res>.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

        internal static void Assert(this IReadOnlyList<string> response, string expected)
        {
            var fault = response.Select(ParseResponse)
                                .FirstOrDefault(x => expected != x);
            if (null != fault)
            {
                throw new ClamException(fault);
            }
        }

        internal static string ParseSingle(this IReadOnlyList<string> response)
        {
            var text = response.SingleOrDefault();
            return null != text ? ParseResponse(text) : string.Empty;
        }

        internal static IDictionary<string, string> ParseMultiple(this IReadOnlyList<string> response)
        {
            var map = response.TakeWhile(x => "END" != x)
                              .Select(x => responsePattern.Match(x))
                              .Where(x => x.Success)
                              .ToDictionary(x => x.Groups["name"].Value, x => x.Groups["res"].Value);
            return map;
        }

        private static string ParseResponse(string response)
        {
            var m = responsePattern.Match(response ?? "");
            return m.Success ? m.Groups["res"].Value : response;
        }
    }
}
