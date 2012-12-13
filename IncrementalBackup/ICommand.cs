using System.Collections.Generic;
namespace IncrementalBackup
{
    public interface ICommand
    {
        void Progress(Dictionary<string, string> namedParameters, string[] parameters);
        string Identifier { get; }
        string Description { get;}
    }
}
