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

            var issuer = CommandHelper.GetProperty(namedParameters, "--issuer", Environment.UserName);
            string comment = CommandHelper.GetProperty(namedParameters, "--comment", "Backup createt at " + DateTime.Now);

            Backup.Create (parameters[0], parameters[1], issuer, comment);
        }

        public string Identifier
        {
            get { return "create"; }
        }

        public string Description
        {
            get
            {
                return @"backup create (--issuer=x) (--comment=x) [source] [destination]

Creates a new backup of the given [source] directory into the [destination] directory. If there is a 1.zip file in it it will use i as the parent backup. This command works recursive.
If issuer is not set it will be set to the current windows user name. If comment is not set it will be ""Backup createt at {current date and time}""";
            }
        }
    }
}
