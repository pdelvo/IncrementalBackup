using System;
using System.Collections.Generic;
using System.IO;
using IncrementalBackup.Library;
using System.Linq;

namespace IncrementalBackup.Commands
{
    public class CreateCommand : ICommand
    {
        public void Progress(Dictionary<string, string> namedParameters, string[] parameters)
        {
            if (parameters.Length != 2)
            {
                throw new ArgumentOutOfRangeException("parameters", "Parameter count must be 2");
            }

            if (!Directory.Exists(parameters[0]))
                throw new ArgumentException("Source directory not found.");
            if (!Directory.Exists(parameters[1]))
                throw new ArgumentException("Destination directory not found.");

            var workingDirectory = new WorkingDirectory(parameters[0]);
            workingDirectory.ImportFiles();

            var backupStatus = new BackupStatus();

            string newId = new Random().Next().ToString();

            if (File.Exists(Path.Combine(parameters[1], "1.zip")))
            {
                File.Move(Path.Combine(parameters[1], "1.zip"), Path.Combine(parameters[1], newId + ".zip"));
            }
            else
            {
                newId = null;
            }

            backupStatus.ReadFiles(Path.Combine(parameters[1], newId + ".zip"), true);

            workingDirectory.CreateIncrementalBackup(Path.Combine(parameters[1], "1.zip"), backupStatus, newId);
        }

        public string Identifier
        {
            get { return "create"; }
        }

        public string Description
        {
            get
            {
                return @"backup create [source] [destination]

Creates a new backup of the given [source] directory into the [destination] directory. If there is a 1.zip file in it it will use i as the parent backup. This command works recursive.";
            }
        }
    }
}
