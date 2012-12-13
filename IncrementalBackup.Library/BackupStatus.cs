using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace IncrementalBackup.Library
{
    public sealed class BackupStatus
    {
        public BackupRoot Root { get; private set; }

        public BackupStatus()
        {
            Root = new BackupRoot ();
        }

        public void ReadFiles(string path, bool recursive = false)
        {
            if (File.Exists(path))
            {
                ReadFilesInternal(path);
                var information = Root.Information;

                while (recursive && !string.IsNullOrEmpty(information.ParentName))
                {
                    ReadFilesInternal(Path.Combine(Path.GetDirectoryName(path), information.ParentName + ".zip"));
                }

                information.DeletedFiles = Root.Information.DeletedFiles;
                Root.Information = information;
            }
            else
            {
                Root.Information = new BackupInformation
                                       {
                                           DeletedFiles = new HashSet<string> (),
                                           ParentName = null
                                       };
            }
        }

        private void ReadFilesInternal(string path)
        {
            using (var file = ZipFile.Open(path, ZipArchiveMode.Read))
            {
                var informationFile = file.GetEntry("info.xml");
                using (var stream = informationFile.Open ())
                {
                    if (Root.Information == null)
                    {
                        Root.Information = BackupInformation.Read(stream);
                    }
                    else
                    {
                        var information = BackupInformation.Read(stream);

                        Root.Information.ParentName = information.ParentName;

                        Root.Information.DeletedFiles.AddRange(information.DeletedFiles);
                    }
                }
                ImportFiles(file, path);
            }
        }

        private void ImportFiles(ZipArchive file, string path)
        {
            var enumerable = file.Entries.Where(a => a.FullName.StartsWith("data/"));

            foreach (var zipArchiveEntry in enumerable)
            {
                var directoryName = Path.GetDirectoryName(zipArchiveEntry.FullName);
                if (directoryName != null)
                {
                    string virtualPath = "." + (directoryName.Replace("\\", "/") + "/" + Path.GetFileNameWithoutExtension(zipArchiveEntry.FullName)).Substring(4);
                    var extension = Path.GetExtension(zipArchiveEntry.FullName);
                    if (extension != null && !Root.Information.DeletedFiles.Contains(virtualPath))
                    {
                        string hash = extension.Substring(1);

                        Root.Children.Add(new BackupFile
                                              {
                                                  Name = Path.GetFileNameWithoutExtension(Path.GetFileName(zipArchiveEntry.FullName)),
                                                  FileHash = hash,
                                                  VirtualPath = virtualPath,
                                                  ArchivePath = path
                                              });
                    }
                }
            }
        }
    }
}
