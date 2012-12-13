using System;
using System.Collections.Generic;
using System.Linq;

namespace IncrementalBackup.Commands
{
    static class CommandHelper
    {
        internal static string GetProperty(Dictionary<string, string> namedParameters, string key, string defaultValue)
        {
            var result = namedParameters.FirstOrDefault(
                a => string.Compare(key, a.Key, StringComparison.InvariantCultureIgnoreCase) == 0);
            return default(KeyValuePair<string, string>).Equals(result) ? defaultValue : result.Value;
        }
    }
}
