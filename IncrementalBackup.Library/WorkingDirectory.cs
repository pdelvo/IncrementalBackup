using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace IncrementalBackup.Library
{
    public class WorkingDirectory
    {
        public string WorkingDirectoryPath { get; set; }

        public WorkingDirectory(string path)
        {
            Root = new BackupRoot ();
            WorkingDirectoryPath = path;
        }

        public BackupRoot Root { get; set; }

        public void ImportFiles()
        {
            var enumerable = Directory.EnumerateFiles(WorkingDirectoryPath, "*.*", SearchOption.AllDirectories);

            Parallel.ForEach(enumerable, entry =>
                                             {
                                                 var directoryName =
                                                     Path.GetDirectoryName(entry).Substring(WorkingDirectoryPath.Length);
                                                 if (directoryName != null)
                                                 {
                                                     string virtualPath = "." +
                                                                          (directoryName.Replace("\\", "/") + "/" +
                                                                           Path.GetFileName(entry));
                                                     var extension = Path.GetExtension(entry);
                                                     if (extension != null)
                                                     {

                                                         Root.Children.Add(new BackupFile
                                                                               {
                                                                                   Name = Path.GetFileName(entry),
                                                                                   FileHash =
                                                                                       FileHelpers.CalculateHash(entry),
                                                                                   VirtualPath = virtualPath,
                                                                                   ArchivePath = WorkingDirectoryPath
                                                                               });
                                                     }
                                                 }
                                             });
        }

        public void CreateIncrementalBackup(string fileName,  BackupStatus backupStatus, string parentHash = null, string comment = null, string issuer = null)
        {
            var addedFiles = Root.Children.Where(
                    a =>
                    !backupStatus.Root.Children.Any(m => m.VirtualPath == a.VirtualPath && m.FileHash == a.FileHash));
            var removedFiles = backupStatus.Root.Children.Where(
                    a =>
                    Root.Children.All(m => m.VirtualPath != a.VirtualPath));

            SaveBackup (fileName, parentHash, removedFiles, addedFiles, comment, issuer);

        }

        private void SaveBackup(string fileName, string parentHash, IEnumerable<BackupFile> removedFiles, IEnumerable<BackupFile> addedFiles, string comment = null, string issuer = null)
        {
            using (var file = ZipFile.Open(fileName, ZipArchiveMode.Create))
            {
                var informationFile = file.CreateEntry("info.xml");
                using (var stream = informationFile.Open ())
                {
                    var information = new BackupInformation
                                          {
                                              DeletedFiles =
                                                  new HashSet<string>(
                                                  removedFiles.Select(a => a.VirtualPath)),
                                              ParentName = parentHash,
                                              CreationDate = DateTime.Now,
                                              Comment = comment,
                                              Issuer = issuer
                                          };
                    information.Save(stream);
                }
                foreach (var backupFile in addedFiles)
                {
                    var relativeName = "data" + backupFile.VirtualPath.Substring(1);
                    var entry = file.CreateEntry(relativeName + "." + backupFile.FileHash);

                    using (var stream = entry.Open ())
                    {
                        using (
                            var fileStream =
                                new FileStream(Path.Combine(WorkingDirectoryPath, backupFile.VirtualPath.Substring(2)),
                                               FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            fileStream.CopyTo(stream);
                        }
                    }
                }
            }
        }
    }
}
