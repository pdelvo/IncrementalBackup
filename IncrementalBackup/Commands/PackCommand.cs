using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IncrementalBackup.Library;

namespace IncrementalBackup.Commands
{
    public class PackCommand : ICommand
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

            Backup.Pack(parameters[0], parameters[1], maxDepth, force);
        }

        public string Identifier
        {
            get { return "pack"; }
        }

        public string Description
        {
            get
            {
                return @"backup pack (--force) (--max-depth=x) [zip] [dir]

Packs a given backup container into a [destination] file. If max depth is specified it will only use x parent files. If not it will use everyone. If force is specified it will override the existing file.";
            }
        }
    }
}
