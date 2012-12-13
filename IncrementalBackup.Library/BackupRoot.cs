using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IncrementalBackup.Library
{
    public sealed class BackupRoot : BackupNode
    {
        public override string Name
        {
            get
            {
                return ".";
            }
            set
            {
                throw new InvalidOperationException("Name of the root element cannot be set.");
            }
        }
        public override string VirtualPath
        {
            get
            {
                return ".";
            }
            set
            {
                throw new InvalidOperationException("Name of the root element cannot be set.");
            }
        }

        public BlockingCollection<BackupFile> Children { get; set; }

        public BackupInformation Information { get; set; }

        public BackupRoot()
        {
            Children = new BlockingCollection<BackupFile>();
        }
    }
}
