using EternalSoundMetadataPatcher.Backups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class BackupStrategyTest
    {
        [TestMethod]
        [DeploymentItem("TestData/Backups/test.txt", "TestLinearBackups")]
        public void TestLinearBackups()
        {
            var strategy = new LinearBackupStrategy();

            Assert.IsFalse(File.Exists(@"TestLinearBackups\test.txt.bak1"));

            strategy.CreateNew(Path.GetFullPath(@"TestLinearBackups\test.txt"));
            Assert.IsTrue(File.Exists(@"TestLinearBackups\test.txt.bak1"));

            strategy.CreateNew(Path.GetFullPath(@"TestLinearBackups\test.txt"));
            Assert.IsTrue(File.Exists(@"TestLinearBackups\test.txt.bak2"));
        }

        [TestMethod]
        [DeploymentItem("TestData/Backups/test.txt", "TestRotateBackups")]
        public void TestRotateBackups()
        {
            var strategy = new RotateBackupStrategy(3);

            Assert.IsFalse(File.Exists(@"TestRotateBackups\test.txt.bak1"));

            strategy.CreateNew(Path.GetFullPath(@"TestRotateBackups\test.txt"));
            Assert.IsTrue(File.Exists(@"TestRotateBackups\test.txt.bak1"));
            var oldWriteTime = File.GetLastWriteTimeUtc(@"TestRotateBackups\test.txt.bak1");

            System.Threading.Thread.Sleep(1000);


            strategy.CreateNew(Path.GetFullPath(@"TestRotateBackups\test.txt"));
            Assert.IsTrue(File.Exists(@"TestRotateBackups\test.txt.bak2"));

            System.Threading.Thread.Sleep(1000);


            strategy.CreateNew(Path.GetFullPath(@"TestRotateBackups\test.txt"));
            Assert.IsTrue(File.Exists(@"TestRotateBackups\test.txt.bak3"));

            System.Threading.Thread.Sleep(1000);

            strategy.CreateNew(Path.GetFullPath(@"TestRotateBackups\test.txt"));
            Assert.IsFalse(File.Exists(@"TestRotateBackups\test.txt.bak4"));
            var newWriteTime = File.GetLastWriteTimeUtc(@"TestRotateBackups\test.txt.bak1");
            Assert.IsTrue(newWriteTime > oldWriteTime);
        }
    }
}
