namespace IncrementalBackup.Library
{
    public class BackupNode
    {
        public virtual string Name { get; set; }
        public virtual string VirtualPath { get; set; }
        public virtual string ArchivePath { get; set; }

    }
}
