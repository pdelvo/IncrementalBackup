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
            var result = namedParameters.FirstOrDefault(
                a => string.Compare("--max-depth", a.Key, StringComparison.InvariantCultureIgnoreCase) == 0);
            int maxDepth = default(KeyValuePair<string, string>).Equals(result) ? 0 : int.Parse(result.Value);

            string zip = parameters[0];
            string resultPath = parameters.Length == 2 ? parameters[1] : Path.GetTempFileName ();
            bool overrideZip = parameters.Length == 1;

            if (!File.Exists(zip))
                throw new ArgumentException("Source directory not found.");
            if (!Directory.Exists(Path.GetDirectoryName(resultPath)))
                throw new ArgumentException("Destination directory not found.");
            if (File.Exists(resultPath))
                throw new ArgumentException("Destination file already exists.");

            var backupStatus = new BackupStatus ();

            backupStatus.ReadFiles(parameters[0], true, maxDepth);


            var groups = from x in backupStatus.Root.Children
                         group x by x.ArchivePath
                         into archives
                         select archives;

            using (var file = ZipFile.Open(resultPath, ZipArchiveMode.Create))
            {
                var informationFile = file.CreateEntry("info.xml");
                using (var stream = informationFile.Open ())
                {
                    var information = new BackupInformation
                                          {
                                              DeletedFiles = backupStatus.LastDeletedFiles,
                                              ParentName = backupStatus.Root.Information.ParentName,
                                              CreationDate = DateTime.Now
                                          };
                    information.Save(stream);
                }
                foreach (var group in groups)
                {
                    using (var zipFile = ZipFile.Open(group.Key, ZipArchiveMode.Read))
                    {
                        foreach (var backupFile in group)
                        {
                            var relativeName = "data" + backupFile.VirtualPath.Substring(1);
                            var entry = file.CreateEntry(relativeName + "." + backupFile.FileHash);

                            using (var stream = entry.Open ())
                            {
                                using (var fileStream = zipFile.GetEntry(entry.FullName).Open ())
                                {
                                    fileStream.CopyTo(stream);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string Identifier
        {
            get { return "combine"; }
        }

        public string Description
        {
            get { return @"backup combine (--max-depth=x) (--delete-parents) [zip] (output)

Combines all parent backup files into one. You can specify a file path the resulting backup file should be placed in. If you don't specify it the old file will be overridden. 
You can set the maximum depth of it by seting the --max-depth option"; }
        }
    }
}
