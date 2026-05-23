using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class AppTests
    {
        public TestContext TestContext { get; set; }

        private void StringsEqualNormalized(string a, string b)
        {
            Assert.AreEqual(
                NormalizeEOL(a),
                NormalizeEOL(b)
            );
        }

        private string NormalizeEOL(string text)
        {
            return text.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        [TestMethod]
        [DeploymentItem("TestData/Comparisons/AppTests/TestHelp.txt", "Comparisons/AppTests")]
        public void TestNoArguments()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            int result = App.Main(new string[] { });

            Assert.AreEqual(0, result);
            StringsEqualNormalized(File.ReadAllText(@"Comparisons\AppTests\TestHelp.txt"), writer.ToString());
        }

        [TestMethod]
        [DeploymentItem("TestData/Comparisons/AppTests/TestHelp.txt", "Comparisons/AppTests")]
        public void TestHelpArgument()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            int result = App.Main(new string[] { "-h" });

            Assert.AreEqual(0, result);
            StringsEqualNormalized(File.ReadAllText(@"Comparisons\AppTests\TestHelp.txt"), writer.ToString());
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
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin", "no_backups/mod/base/sound/soundbanks/pc")]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_orig.xml", "no_backups/mod/base/sound/soundbanks")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "no_backups/wwise")]
        public void TestBackupArgumentNoBackups()
        {
            var modPath = @"no_backups\mod";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"no_backups\wwise\ModsWwise";

            File.Move(
                Path.Combine(bnksPath, @"pc\soundmetadata_orig.bin"),
                Path.Combine(bnksPath, @"pc\soundmetadata.bin")
            );
            File.Move(
                Path.Combine(bnksPath, "SoundbanksInfo_orig.xml"),
                Path.Combine(bnksPath, "SoundbanksInfo.xml")
            );

            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1")));

            int result = App.Main(new string[] { "-b", "0", modPath, wwisePath });

            Assert.AreEqual(0, result);
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1")));
        }

        [TestMethod]
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin", "linear_backups/mod/base/sound/soundbanks/pc")]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_orig.xml", "linear_backups/mod/base/sound/soundbanks")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "linear_backups/wwise")]
        public void TestBackupArgumentLinearBackups()
        {
            var modPath = @"linear_backups\mod";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"linear_backups\wwise\ModsWwise";

            File.Move(
                Path.Combine(bnksPath, @"pc\soundmetadata_orig.bin"),
                Path.Combine(bnksPath, @"pc\soundmetadata.bin")
            );
            File.Move(
                Path.Combine(bnksPath, "SoundbanksInfo_orig.xml"),
                Path.Combine(bnksPath, "SoundbanksInfo.xml")
            );

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
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin", "rotate_backups/mod/base/sound/soundbanks/pc")]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_orig.xml", "rotate_backups/mod/base/sound/soundbanks")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "rotate_backups/wwise")]
        public void TestBackupArgumentRotateBackups()
        {
            var modPath = @"rotate_backups\mod";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"rotate_backups\wwise\ModsWwise";

            File.Move(
                Path.Combine(bnksPath, @"pc\soundmetadata_orig.bin"),
                Path.Combine(bnksPath, @"pc\soundmetadata.bin")
            );
            File.Move(
                Path.Combine(bnksPath, "SoundbanksInfo_orig.xml"),
                Path.Combine(bnksPath, "SoundbanksInfo.xml")
            );

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
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin", "default_output/mod/base/sound/soundbanks/pc")]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_orig.xml", "default_output/mod/base/sound/soundbanks")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "default_output/wwise")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestDefaultOutput.txt", "Comparisons/AppTests")]
        public void TestDefaultOutput()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"default_output\mod";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"default_output\wwise\ModsWwise";

            File.Move(
                Path.Combine(bnksPath, @"pc\soundmetadata_orig.bin"),
                Path.Combine(bnksPath, @"pc\soundmetadata.bin")
            );
            File.Move(
                Path.Combine(bnksPath, "SoundbanksInfo_orig.xml"),
                Path.Combine(bnksPath, "SoundbanksInfo.xml")
            );

            int result = App.Main(new string[] { modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestDefaultOutput.txt")
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringsEqualNormalized(expected, writer.ToString());
        }

        [TestMethod]
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin", "verbose_output/mod/base/sound/soundbanks/pc")]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_orig.xml", "verbose_output/mod/base/sound/soundbanks")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "verbose_output/wwise")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestVerboseOutput.txt", "Comparisons/AppTests")]
        public void TestVerboseOutput()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"verbose_output\mod";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"verbose_output\wwise\ModsWwise";

            File.Move(
                Path.Combine(bnksPath, @"pc\soundmetadata_orig.bin"),
                Path.Combine(bnksPath, @"pc\soundmetadata.bin")
            );
            File.Move(
                Path.Combine(bnksPath, "SoundbanksInfo_orig.xml"),
                Path.Combine(bnksPath, "SoundbanksInfo.xml")
            );

            int result = App.Main(new string[] { "-v", modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestVerboseOutput.txt")
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringsEqualNormalized(expected, writer.ToString());
        }

        [TestMethod]
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin", "debug_output/mod/base/sound/soundbanks/pc")]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_orig.xml", "debug_output/mod/base/sound/soundbanks")]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "debug_output/wwise")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestDebugOutput.txt", "Comparisons/AppTests")]
        public void TestDebugOutput()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"debug_output\mod";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"debug_output\wwise\ModsWwise";

            File.Move(
                Path.Combine(bnksPath, @"pc\soundmetadata_orig.bin"),
                Path.Combine(bnksPath, @"pc\soundmetadata.bin")
            );
            File.Move(
                Path.Combine(bnksPath, "SoundbanksInfo_orig.xml"),
                Path.Combine(bnksPath, "SoundbanksInfo.xml")
            );

            int result = App.Main(new string[] { "-d", modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestDebugOutput.txt")
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringsEqualNormalized(expected, writer.ToString());
        }
    }
}
