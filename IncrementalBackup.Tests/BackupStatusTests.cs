using System;
using System.Globalization;
using IncrementalBackup.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace IncrementalBackup.Tests
{
    [TestClass]
    public class BackupStatusTests
    {
        public string BackupDirectory { get; set; }

        [TestInitialize]
        public void InitializeTests()
        {
            var testPath = Path.Combine(Path.GetTempPath (), new Random ().Next ().ToString (CultureInfo.InvariantCulture));
            Directory.CreateDirectory(testPath);
            BackupDirectory = testPath;

            CreateTestBackupFile ();
        }

        private void CreateTestBackupFile()
        {
            using (var file = ZipFile.Open(Path.Combine(BackupDirectory, "test.zip"), ZipArchiveMode.Create))
            {
                var testData = file.CreateEntry("data/test.txt.123456789");
                using (var stream = testData.Open())
                {
                    stream.WriteByte(1);
                }
                testData = file.CreateEntry("data/test2.txt.123456789");
                using (var stream = testData.Open())
                {
                    stream.WriteByte(2);
                }
                testData = file.CreateEntry("data/bin/test3.txt.123456789");
                using (var stream = testData.Open())
                {
                    stream.WriteByte(3);
                }
                testData = file.CreateEntry("data/bin/a/b/c/test4.txt.123456789");
                using (var stream = testData.Open())
                {
                    stream.WriteByte(4);
                }
                testData = file.CreateEntry("info.xml");
                using (var stream = testData.Open())
                {
                    const string infoData = @"<?xml version=""1.0""?>
                                    <BackupInformation xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                                        <DeletedFiles>
                                            <string>./test_del.txt</string>
                                        </DeletedFiles>
                                        <ParentName>248990293</ParentName>
                                    </BackupInformation > ";
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(infoData);
                    }
                }
            }
        }

        [TestCleanup]
        public void CleanupTest()
        {
            Directory.Delete(BackupDirectory, true);
        }

        [TestMethod]
        public void BackupStatusHasRootNode()
        {
            var backupStatus = new BackupStatus();

            Assert.IsNotNull(backupStatus.Root);
        }

        [TestMethod]
        public void BackupStatusReadSingleFileCorrectly()
        {
            var backupStatus = new BackupStatus();

            backupStatus.ReadFiles(Path.Combine(BackupDirectory, "test.zip"));

            CollectionAssert.AllItemsAreUnique(backupStatus.Root.Children);

            Assert.AreEqual(4, backupStatus.Root.Children.Count);

            Assert.IsNotNull(backupStatus.Root.Children.SingleOrDefault(a => a.VirtualPath == "./test.txt"));
            Assert.IsNotNull(backupStatus.Root.Children.SingleOrDefault(a => a.VirtualPath == "./test2.txt"));
            Assert.IsNotNull(backupStatus.Root.Children.SingleOrDefault(a => a.VirtualPath == "./bin/test3.txt"));
            Assert.IsNotNull(backupStatus.Root.Children.SingleOrDefault(a => a.VirtualPath == "./bin/a/b/c/test4.txt"));

            var info = backupStatus.Root.Information;

            Assert.IsNotNull(info);
            Assert.IsNotNull(info.DeletedFiles);

            Assert.IsNotNull(info.DeletedFiles.SingleOrDefault(a => a == "./test_del.txt"));
            Assert.AreEqual("248990293", info.ParentName);
        }
    }
}
