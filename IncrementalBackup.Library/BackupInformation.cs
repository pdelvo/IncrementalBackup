using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IncrementalBackup.Library
{
    [Serializable]
    public class BackupInformation
    {
        public HashSet<string> DeletedFiles { get; set; }
        public string ParentName { get; set; }
        public string Comment { get; set; }
        public string Issuer { get; set; }
        public DateTime CreationDate { get; set; }

        public static BackupInformation Read(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(BackupInformation));
            return (BackupInformation) serializer.Deserialize(stream);
        }

        public void Save(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(BackupInformation));
            serializer.Serialize(stream, this);
        }
    }
}
