using System.Collections.Generic;
using System.Linq;
using IncrementalBackup.Commands;
using IncrementalBackup.Library;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace IncrementalBackup
{
    internal class Program
    {
        private static void Main(string[] parameters)
        {
            #region old

            //if (parameters.Length == 2)
            //{
            //    var workingDirectory = new WorkingDirectory(parameters[0]);
            //    workingDirectory.ImportFiles ();

            //    var backupStatus = new BackupStatus ();

            //    string newId = new Random ().Next ().ToString ();

            //    if (File.Exists(Path.Combine(parameters[1], "1.zip")))
            //    {
            //        File.Move(Path.Combine(parameters[1], "1.zip"), Path.Combine(parameters[1], newId + ".zip"));
            //    }
            //    else
            //    {
            //        newId = null;
            //    }

            //    backupStatus.ReadFiles(Path.Combine(parameters[1], newId + ".zip"), true);

            //    workingDirectory.CreateIncrementalBackup(Path.Combine(parameters[1], "1.zip"), backupStatus, newId);
            //}
            //else if (parameters.Length == 3 && parameters[0] == "/u")
            //{
            //    var backupStatus = new BackupStatus ();

            //    backupStatus.ReadFiles(parameters[1], true);


            //    var groups = from x in backupStatus.Root.Children
            //                 group x by x.ArchivePath
            //                 into archives
            //                 select archives;

            //    foreach (var group in groups)
            //    {
            //        using (var archive = ZipFile.Open(group.Key, ZipArchiveMode.Read))
            //        {
            //            foreach (var backupFile in group)
            //            {
            //                var file = "data" + backupFile.VirtualPath.Substring(1) + "." + backupFile.FileHash;

            //                var compressedFile = archive.GetEntry(file);
            //                using (var stream = compressedFile.Open ())
            //                {
            //                    var resultFileName = Path.Combine(parameters[2], backupFile.VirtualPath.Substring(2));

            //                    Directory.CreateDirectory(Path.GetDirectoryName(resultFileName));

            //                    using (var fileStream = File.Create(resultFileName))
            //                    {
            //                        stream.CopyTo(fileStream);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    Console.WriteLine(
            //        "Usage: IncrementalBackup.exe [workingDirectory] [backupDirectory]\r\nIncrementalBackup.exe /u [archive] [resultDirectory]");
            //}

        #endregion

            PrintHeader ();

            var commands = GetCommands();

            if (parameters.Length < 1)
            {
                //TODO: List commands
            }
            else
            {
                var command =
                    commands.FirstOrDefault(
                        a => String.Compare(parameters[0], a.Identifier, StringComparison.InvariantCultureIgnoreCase) == 0);

                if (command == null)
                {
                    //Print options
                }
                else
                {
                    var optionalParameters = new Dictionary<string, string> ();

                    var explicitParameters = new List<string> ();

                    foreach (var item in parameters.Skip(1))
                    {
                        if (item.StartsWith("--"))
                        {
                            var splittedItems = item.Split(new char[] {'='}, 1);

                            optionalParameters.Add(splittedItems[0], splittedItems.Length > 1 ? splittedItems[1] : null);
                        }
                        else
                        {
                            explicitParameters.Add(item);
                        }
                    }
                    try
                    {
                        command.Progress(optionalParameters, explicitParameters.ToArray());
                    }
                    catch (Exception ex)
                    {
                        var color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(ex.Message);
                        Console.ForegroundColor = color;
                    }
                }
            }
        }

        private static void PrintHeader()
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"Incremental Backup by pdelvo, Version {0}", Assembly.GetExecutingAssembly ().GetName().Version);
            Console.ForegroundColor = color;
        }

        private static IEnumerable<ICommand> GetCommands()
        {
            yield return new CreateCommand ();
        }
    }
}
