using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using IncrementalBackup.Library;

namespace IncrementalBackup.Commands
{
    public class ExtractCommand : ICommand
    {
        public void Progress(Dictionary<string, string> namedParameters, string[] parameters)
        {
            if (parameters.Length != 2)
            {
                throw new ArgumentOutOfRangeException("parameters", "Parameter count must be 2");
            }

            bool force =
                namedParameters.Any(a => string.Compare("--force", a.Key, StringComparison.InvariantCultureIgnoreCase) == 0);
            var result = namedParameters.FirstOrDefault(
                a => string.Compare("--max-depth", a.Key, StringComparison.InvariantCultureIgnoreCase) == 0);
            int maxDepth = default(KeyValuePair<string, string>).Equals(result) ? 0 : int.Parse(result.Value);

            Backup.Extract (parameters[0], parameters[1], maxDepth, force);
        }

        public string Identifier
        {
            get { return "extract"; }
        }

        public string Description
        {
            get { return @"backup extract (--force) (--max-depth=x) [zip] [dir]

Extracts a given backup container into a [destination] directory. If max depth is specified it will only use x parent files. If not it will use everyone. If force is specified it will override existing files.";
            }
        }
    }
}