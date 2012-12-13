using System;
using System.Globalization;
using System.IO;
using System.Linq;
using IncrementalBackup.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IncrementalBackup.Tests
{
    [TestClass]
    public class WorkingDirectoryTests
    {
        public string WorkingDirectory { get; set; }
        public TestContext TestContext { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            var testPath = Path.Combine(Path.GetTempPath(), new Random().Next().ToString(CultureInfo.InvariantCulture));
            Directory.CreateDirectory(testPath);
            WorkingDirectory = testPath;

            CreateTestData ();
        }

        private void CreateTestData()
        {
            File.WriteAllText(Path.Combine(WorkingDirectory, "test1.txt"), "test1");
            File.WriteAllText(Path.Combine(WorkingDirectory, "test2.txt"), "test2");
            File.WriteAllText(Path.Combine(WorkingDirectory, "test3.txt"), "test3");

            Directory.CreateDirectory(Path.Combine(WorkingDirectory, "a/b"));
            Directory.CreateDirectory(Path.Combine(WorkingDirectory, "b"));

            File.WriteAllText(Path.Combine(WorkingDirectory, "a/test4.txt"), "test4");
            File.WriteAllText(Path.Combine(WorkingDirectory, "b/test5.txt"), "test5");
            File.WriteAllText(Path.Combine(WorkingDirectory, "a/b/test6.txt"), "test6");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(WorkingDirectory, true);
        }

        [TestMethod]
        public void WorkingDirectoryRootIsNotNull()
        {
            var directory = new WorkingDirectory(WorkingDirectory);

            Assert.IsNotNull(directory.Root);
        }

        [TestMethod]
        public void WorkingDirectoryPathPropertyMatchesConstructorParameter()
        {
            var directory = new WorkingDirectory(WorkingDirectory);

            Assert.AreEqual(WorkingDirectory, directory.WorkingDirectoryPath);
        }

        [TestMethod]
        public void WorkingDirectoryScannerScansDirectoryCorrectly()
        {
            var directory = new WorkingDirectory(WorkingDirectory);

            directory.ImportFiles ();

            Assert.IsNotNull(directory.Root.Children.SingleOrDefault(a => a.VirtualPath == "./test1.txt"));
            Assert.IsNotNull(directory.Root.Children.SingleOrDefault(a => a.VirtualPath == "./test2.txt"));
            Assert.IsNotNull(directory.Root.Children.SingleOrDefault(a => a.VirtualPath == "./test3.txt"));
            Assert.IsNotNull(directory.Root.Children.SingleOrDefault(a => a.VirtualPath == "./a/test4.txt"));
            Assert.IsNotNull(directory.Root.Children.SingleOrDefault(a => a.VirtualPath == "./b/test5.txt"));
            Assert.IsNotNull(directory.Root.Children.SingleOrDefault(a => a.VirtualPath == "./a/b/test6.txt"));

            foreach (var backupFile in directory.Root.Children)
            {
                Assert.IsNotNull(backupFile.FileHash);
                Assert.AreEqual(WorkingDirectory, backupFile.ArchivePath);
                Assert.IsNotNull(backupFile.VirtualPath);
                Assert.IsNotNull(backupFile.Name);
            }
        }
    }
}
