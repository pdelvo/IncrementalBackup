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
        internal static ICommand[] Commands;

        private static void Main(string[] parameters)
        {

            PrintHeader ();

            var commands = Commands = GetCommands().ToArray ();

            if (parameters.Length < 1)
            {
                new HelpCommand().Progress(null, new string[0]);
            }
            else
            {
                var command =
                    commands.FirstOrDefault(
                        a => String.Compare(parameters[0], a.Identifier, StringComparison.InvariantCultureIgnoreCase) == 0);

                if (command == null)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Command not found");
                    Console.ForegroundColor = color;
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
            yield return new HelpCommand();
            yield return new CreateCommand();
            yield return new ExtractCommand();
        }
    }
}
