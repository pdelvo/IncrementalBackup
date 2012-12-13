using System;
using System.Collections.Generic;
using System.IO;
using IncrementalBackup.Library;

namespace IncrementalBackup.Commands
{
    public class InfoCommand : ICommand
    {
        public void Progress(Dictionary<string, string> namedParameters, string[] parameters)
        {
            if (parameters.Length != 1)
                throw new ArgumentOutOfRangeException("parameters", "Parameter count must be one");
            if (!File.Exists(parameters[0]))
                throw new ArgumentException("Source directory not found.");

            var backup = new BackupStatus ();
            backup.ReadFiles(parameters[0]);

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Path.GetFileName(parameters[0]) + ": ");
            Console.ForegroundColor = color;

            Console.WriteLine ();

            var mode = CommandHelper.GetProperty(namedParameters, "--mode", "");

            if (mode.ToLower () == "added")
            {
                color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Magenta;
                foreach (var backupFile in backup.Root.Children)
                {
                    Console.WriteLine("{0}; {1}", backupFile.VirtualPath, backupFile.FileHash);
                }
                Console.ForegroundColor = color;
            }
            else if (mode.ToLower () == "deleted")
            {
                color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Magenta;
                foreach (var backupFile in backup.Root.Information.DeletedFiles)
                {
                    Console.WriteLine( backupFile);
                }
                Console.ForegroundColor = color;
            }
            else
            {
                color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("{0}: {1}", "Issuer", backup.Root.Information.Issuer);
                Console.WriteLine("{0}: {1}", "Comment", backup.Root.Information.Comment);
                Console.WriteLine("{0}: {1}", "Deleted Files", backup.Root.Information.DeletedFiles.Count);
                Console.WriteLine("{0}: {1}", "Added or modified Files", backup.Root.Children.Count);
                Console.WriteLine("{0}: {1}", "Creation Date", backup.Root.Information.CreationDate);
                Console.ForegroundColor = color;
            }
        }

        public string Identifier
        {
            get { return "info"; }
        }

        public string Description
        {
            get { return @"backup info (--mode=mode) [zip]

Prints information about the given backup file. mode can be 'added' for a list of added or modified files or deleted for deleted files in this backup.";
            }
        }
    }
}
