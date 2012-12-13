using System.IO;
using System.Linq;

namespace IncrementalBackup.Library
{
    internal static class FileHelpers
    {
        internal static string CalculateHash(string path)
        {
            var provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var inFileInfo = new FileInfo(path);
            using (var inStream = new FileStream(inFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var hashBytes = provider.ComputeHash(inStream);
                return hashBytes.Aggregate("", (current, inByte) => current + inByte.ToString("X2"));
            }
        }
    }
}
