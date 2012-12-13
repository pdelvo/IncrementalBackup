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

            if (!File.Exists(parameters[0]))
                throw new ArgumentException("Source directory not found.");
            if (!Directory.Exists(parameters[1]))
                throw new ArgumentException("Destination directory not found.");

            var backupStatus = new BackupStatus();

            backupStatus.ReadFiles(parameters[0], true, maxDepth);


            var groups = from x in backupStatus.Root.Children
                         group x by x.ArchivePath
                             into archives
                             select archives;

            Parallel.ForEach(groups, group =>
                                         {
                                             using (var archive = ZipFile.Open(group.Key, ZipArchiveMode.Read))
                                             {
                                                 foreach (var backupFile in group)
                                                 {
                                                     var file = "data" + backupFile.VirtualPath.Substring(1) + "." +
                                                                backupFile.FileHash;

                                                     var compressedFile = archive.GetEntry(file);
                                                     using (var stream = compressedFile.Open ())
                                                     {
                                                         var resultFileName = Path.Combine(parameters[1],
                                                                                           backupFile.VirtualPath
                                                                                                     .Substring(2));

                                                         Directory.CreateDirectory(Path.GetDirectoryName(resultFileName));

                                                         if (File.Exists(resultFileName) && !force)
                                                             Console.WriteLine("Overriding files is disabled. Use --force to enable it.");
                                                         else if(force)
                                                             File.Delete(resultFileName);

                                                         using (var fileStream = File.Create(resultFileName))
                                                         {
                                                             stream.CopyTo(fileStream);
                                                         }
                                                     }
                                                 }
                                             }
                                         });
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