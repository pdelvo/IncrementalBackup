using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IncrementalBackup.Commands
{
    public class HelpCommand : ICommand
    {
        public void Progress(Dictionary<string, string> namedParameters, string[] parameters)
        {
            var color = Console.ForegroundColor;
            if (parameters.Length == 0)
            {
                color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Here is a list of supported commands: ");
                Console.ForegroundColor = color;

                foreach (var command in Program.Commands)
                {
                    color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("{0}", command.Identifier);
                    Console.ForegroundColor = color;
                }
                color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("---------------------");
                Console.WriteLine("Type help [command] for more information about a command");
                Console.ForegroundColor = color;
            }
            else if(parameters.Length == 1)
            {
                var command = Program.Commands.SingleOrDefault(a => a.Identifier.ToLower () == parameters[0].ToLower ());

                if (command == null)
                {
                    color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Command not found");
                    Console.ForegroundColor = color;
                }
                else
                {
                    color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0}: ", command.Identifier);
                    Console.ForegroundColor = color;
                    color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(command.Description);
                    Console.ForegroundColor = color;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("---------------------");
                    Console.WriteLine("Type help for a list of avialable commands");
                    Console.ForegroundColor = color;
                }
            }
        }

        public string Identifier
        {
            get { return "help"; }
        }

        public string Description
        {
            get { return "Print information about commands"; }
        }
    }
}
