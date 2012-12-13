using IncrementalBackup.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IncrementalBackup.Tests
{
    [TestClass]
    public class BackupNodeTests
    {
        [TestMethod]
        public void BackupRootHasInitializedElementsList()
        {
            var root = new BackupRoot();

            Assert.IsNotNull(root.Children);
        }

        [TestMethod]
        public void BackupRootNamePropertyTest()
        {
            var root = new BackupRoot();

            Assert.AreEqual(root.Name, ".");

            TestExtensions.Throws<InvalidOperationException>(() => root.Name = "Test");
        }

        [TestMethod]
        public void BackupRootVirtualPathPropertyTest()
        {
            var root = new BackupRoot();

            Assert.AreEqual(root.VirtualPath, ".");

            TestExtensions.Throws<InvalidOperationException>(() => root.VirtualPath = "Test");
        }
    }
}
