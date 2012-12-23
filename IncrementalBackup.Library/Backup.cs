using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace IncrementalBackup.Library
{
    public static class Backup
    {
        public static void Create(string source, string destination, string issuer, string comment)
        {
            if (!Directory.Exists(source))
                throw new ArgumentException("Source directory not found.");
            if (!Directory.Exists(destination))
                throw new ArgumentException("Destination directory not found.");

            var workingDirectory = new WorkingDirectory(source);
            workingDirectory.ImportFiles();

            var backupStatus = new BackupStatus();

            string newId = new Random().Next().ToString();

            if (File.Exists(Path.Combine(destination, "1.zip")))
            {
                File.Move(Path.Combine(destination, "1.zip"), Path.Combine(destination, newId + ".zip"));
            }
            else
            {
                newId = null;
            }

            backupStatus.ReadFiles(Path.Combine(destination, newId + ".zip"), true);

            workingDirectory.CreateIncrementalBackup(Path.Combine(destination, "1.zip"), backupStatus, newId, comment, issuer);
        }
        public static void Extract(string source, string destination, int maxDepth, bool force)
        {
            if (!File.Exists(source))
                throw new ArgumentException("Source directory not found.");
            if (!Directory.Exists(destination))
                throw new ArgumentException("Destination directory not found.");

            var backupStatus = new BackupStatus();

            backupStatus.ReadFiles(source, true, maxDepth);


            var groups = from x in backupStatus.Root.Children
                         group x by x.ArchivePath
                             into archives
                             select archives;

            Parallel.ForEach(groups, group =>
            {
                using (var archive = ZipFile.Open(@group.Key, ZipArchiveMode.Read))
                {
                    foreach (var backupFile in @group)
                    {
                        var file = "data" + backupFile.VirtualPath.Substring(1) + "." +
                                   backupFile.FileHash;

                        var compressedFile = archive.GetEntry(file);
                        using (var stream = compressedFile.Open())
                        {
                            var resultFileName = Path.Combine(destination,
                                                              backupFile.VirtualPath
                                                                        .Substring(2));

                            Directory.CreateDirectory(Path.GetDirectoryName(resultFileName));

                            if (File.Exists(resultFileName) && !force)
                                Console.WriteLine(
                                    "Overriding files is disabled. Use --force to enable it.");
                            else if (force)
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

        public static void Pack(string source, string destination, int maxDepth, bool force)
        {
            if (!File.Exists(source))
                throw new ArgumentException("Source directory not found.");

            if (File.Exists(destination) && !force)
                Console.WriteLine(
                    "Overriding files is disabled. Use --force to enable it.");
            else if (force)
                File.Delete(destination);

            var backupStatus = new BackupStatus ();

            backupStatus.ReadFiles(source, true, maxDepth);


            var groups = from x in backupStatus.Root.Children
                         group x by x.ArchivePath
                         into archives
                         select archives;
            using (var resultArchive = ZipFile.Open(destination, ZipArchiveMode.Create))
            {
                foreach (var group in groups)
                {
                    using (var archive = ZipFile.Open(@group.Key, ZipArchiveMode.Read))
                    {
                        foreach (var backupFile in @group)
                        {
                            var file = "data" + backupFile.VirtualPath.Substring(1) + "." +
                                       backupFile.FileHash;

                            var compressedFile = archive.GetEntry(file);
                            using (var stream = compressedFile.Open ())
                            {
                                var resultFileName = backupFile.VirtualPath.Substring(2);
                                var entry = resultArchive.CreateEntry(resultFileName);
                                using (var fileStream = entry.Open ())
                                {
                                    stream.CopyTo(fileStream);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Combine(string zipPath, string resultPath, int maxDepth, string issuer,
                                   string comment)
        {
            if (!File.Exists(zipPath))
                throw new ArgumentException("Source directory not found.");
            if (!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(resultPath))))
                throw new ArgumentException("Destination directory not found.");
            if (File.Exists(resultPath))
                throw new ArgumentException("Destination file already exists.");

            var backupStatus = new BackupStatus();

            backupStatus.ReadFiles(zipPath, true, maxDepth);


            var groups = from x in backupStatus.Root.Children
                         group x by x.ArchivePath
                         into archives
                         select archives;

            using (var file = ZipFile.Open(resultPath, ZipArchiveMode.Create))
            {
                var informationFile = file.CreateEntry("info.xml");
                using (var stream = informationFile.Open())
                {
                    var information = new BackupInformation
                                          {
                                              DeletedFiles = backupStatus.LastDeletedFiles,
                                              ParentName = backupStatus.Root.Information.ParentName,
                                              CreationDate = DateTime.Now,
                                              Issuer = issuer,
                                              Comment = comment
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

                            using (var stream = entry.Open())
                            {
                                using (var fileStream = zipFile.GetEntry(entry.FullName).Open())
                                {
                                    fileStream.CopyTo(stream);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<BackupFile> GetFiles(string source)
        {
            var backup = new BackupStatus();
            backup.ReadFiles(source);
            return backup.Root.Children;
        }

        public static IEnumerable<string> GetDeletedFiles(string source)
        {
            var backup = new BackupStatus();
            backup.ReadFiles(source);
            return backup.Root.Information.DeletedFiles;
        }

        public static BackupStatus GetStatus(string path)
        {
            var backup = new BackupStatus();
            backup.ReadFiles(path);
            return backup;
        }
 
    }
}
