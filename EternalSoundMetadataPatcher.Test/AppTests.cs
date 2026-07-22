using EternalSoundMetadataPatcher.Test.TestTools.Assertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class AppTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("TestData/Comparisons/AppTests/TestHelp.txt", "Comparisons/AppTests")]
        public void TestNoArguments()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            int result = App.Main(new string[] { });

            Assert.AreEqual(0, result);
            StringAssertion.AreEqualNormalized(File.ReadAllText(@"Comparisons\AppTests\TestHelp.txt"), writer.ToString());
        }

        [TestMethod]
        [DeploymentItem("TestData/Comparisons/AppTests/TestHelp.txt", "Comparisons/AppTests")]
        public void TestHelpArgument()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            int result = App.Main(new string[] { "-h" });

            Assert.AreEqual(0, result);
            StringAssertion.AreEqualNormalized(File.ReadAllText(@"Comparisons\AppTests\TestHelp.txt"), writer.ToString());
        }

        [TestMethod]
        public void TestBackupArgumentMissingValue()
        {
            var writer = new StringWriter();
            Console.SetError(writer);

            int result = App.Main(new string[] { "-b" });

            Assert.AreEqual(1, result);
            Assert.AreEqual("ERROR: -b argument value missing\r\n", writer.ToString());
        }

        [TestMethod]
        public void TestBackupArgumentInvalidValue()
        {
            var writer = new StringWriter();
            Console.SetError(writer);

            int result = App.Main(new string[] { "-b", "invalid" });

            Assert.AreEqual(1, result);
            Assert.AreEqual("ERROR: -b argument value invalid\r\n", writer.ToString());
        }

        [TestMethod]
        public void TestBackupArgumentValueOutOfRange()
        {
            var writer = new StringWriter();
            Console.SetError(writer);

            int result = App.Main(new string[] { "-b", "-2" });

            Assert.AreEqual(1, result);
            Assert.AreEqual("ERROR: -b argument value out of range\r\n", writer.ToString());
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/NoBackups/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "AppTests/NoBackups/Wwise/Custom")]
        public void TestBackupArgumentNoBackups()
        {
            var modPath = @"AppTests\NoBackups\Mod\Default";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"AppTests\NoBackups\Wwise\Custom\ModsWwise";

            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1")));

            int result = App.Main(new string[] { "-b", "0", modPath, wwisePath });

            Assert.AreEqual(0, result);
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1")));
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/LinearBackups/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "AppTests/LinearBackups/Wwise/Custom")]
        public void TestBackupArgumentLinearBackups()
        {
            var modPath = @"AppTests\LinearBackups\Mod\Default";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"AppTests\LinearBackups\Wwise\Custom\ModsWwise";

            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1")));
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak2")));
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak3")));

            int result = App.Main(new string[] { "-b", "-1", modPath, wwisePath });

            Assert.AreEqual(0, result);
            Assert.IsTrue(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1")));
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak2")));
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak3")));

            result = App.Main(new string[] { "-b", "-1", modPath, wwisePath });

            Assert.AreEqual(0, result);
            Assert.IsTrue(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak2")));
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak3")));

            result = App.Main(new string[] { "-b", "-1", modPath, wwisePath });

            Assert.AreEqual(0, result);
            Assert.IsTrue(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak3")));
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/RotateBackups/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "AppTests/RotateBackups/Wwise/Custom")]
        public void TestBackupArgumentRotateBackups()
        {
            var modPath = @"AppTests\RotateBackups\Mod\Default";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"AppTests\RotateBackups\Wwise\Custom\ModsWwise";

            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1")));
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak2")));
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak3")));

            int result = App.Main(new string[] { "-b", "2", modPath, wwisePath });

            Assert.AreEqual(0, result);
            Assert.IsTrue(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1")));
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak2")));

            var oldWriteTime = File.GetLastWriteTimeUtc(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1"));

            System.Threading.Thread.Sleep(1000);

            result = App.Main(new string[] { "-b", "2", modPath, wwisePath });

            Assert.AreEqual(0, result);
            Assert.IsTrue(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak2")));
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak3")));

            System.Threading.Thread.Sleep(1000);

            result = App.Main(new string[] { "-b", "2", modPath, wwisePath });

            Assert.AreEqual(0, result);
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak3")));
            var newWriteTime = File.GetLastWriteTimeUtc(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1"));
            Assert.IsTrue(newWriteTime > oldWriteTime);
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/TestDefaultOutput/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "AppTests/TestDefaultOutput/Wwise/Custom")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestDefaultOutput.txt", "Comparisons/AppTests")]
        public void TestDefaultOutput()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"AppTests\TestDefaultOutput\Mod\Default";
            var wwisePath = @"AppTests\TestDefaultOutput\Wwise\Custom\ModsWwise";

            int result = App.Main(new string[] { modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestDefaultOutput.txt")
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringAssertion.AreEqualNormalized(expected, writer.ToString());
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/TestVerboseOutput/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "AppTests/TestVerboseOutput/Wwise/Custom")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestVerboseOutput.txt", "Comparisons/AppTests")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestVerboseOutput.txt", "Comparisons/AppTests")]
        public void TestVerboseOutput()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"AppTests\TestVerboseOutput\Mod\Default";
            var wwisePath = @"AppTests\TestVerboseOutput\Wwise\Custom\ModsWwise";

            int result = App.Main(new string[] { "-v", modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestVerboseOutput.txt")
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringAssertion.AreEqualNormalized(expected, writer.ToString());
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/TestDebugOutput/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "AppTests/TestDebugOutput/Wwise/Custom")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestDebugOutput.txt", "Comparisons/AppTests")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestDebugOutput.txt", "Comparisons/AppTests")]
        public void TestDebugOutput()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"AppTests\TestDebugOutput\Mod\Default";
            var wwisePath = @"AppTests\TestDebugOutput\Wwise\Custom\ModsWwise";

            int result = App.Main(new string[] { "-d", modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestDebugOutput.txt")
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringAssertion.AreEqualNormalized(expected, writer.ToString());
        }
    }
}
