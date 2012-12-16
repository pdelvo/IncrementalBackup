using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IncrementalBackup.Library;

namespace IncrementalBackup.Commands
{
    public class CombineCommand : ICommand
    {
        public void Progress(Dictionary<string, string> namedParameters, string[] parameters)
        {
            if (parameters.Length != 2 && parameters.Length != 1)
            {
                throw new ArgumentOutOfRangeException("parameters", "Parameter count must be at least one");
            }

            bool deleteParents =
                namedParameters.Any(
                    a => string.Compare("--delete-parents", a.Key, StringComparison.InvariantCultureIgnoreCase) == 0);

            string maxDepthResult = CommandHelper.GetProperty(namedParameters, "--max-depth", null);
            int maxDepth = maxDepthResult == null ? 0 : int.Parse(maxDepthResult);
            var issuer = CommandHelper.GetProperty (namedParameters, "--issuer", Environment.UserName);
            string comment = CommandHelper.GetProperty(namedParameters, "--comment", "Backup combination");

            string zip = parameters[0];
            string resultPath = parameters.Length == 2 ? parameters[1] : Path.GetTempFileName ();
            bool overrideZip = parameters.Length == 1;

            Backup.Combine(zip, resultPath, maxDepth, issuer, comment);

            if (overrideZip)
                File.Move(zip, resultPath);
        }

        public string Identifier
        {
            get { return "combine"; }
        }

        public string Description
        {
            get { return @"backup combine (--max-depth=x) (--delete-parents) (--issuer=x) (--comment=x) [zip] (output)

Combines all parent backup files into one. You can specify a file path the resulting backup file should be placed in. If you don't specify it the old file will be overridden. 
You can set the maximum depth of it by seting the --max-depth option. If issuer is not set it will be set to the current windows user name. 
If comment is not set it will be ""Backup combination"""; }
        }
    }
}
