using EternalSoundMetadataPatcher.Metadata;
using EternalSoundMetadataPatcher.Test.TestTools.Assertion;
using EternalSoundMetadataPatcher.Test.TestTools.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

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
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "AppTests/NoBackups/Wwise/Default")]
        public void TestBackupArgumentNoBackups()
        {
            var modPath = @"AppTests\NoBackups\Mod\Default";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"AppTests\NoBackups\Wwise\Default\ModsWwise";

            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1")));

            int result = App.Main(new string[] { "-b", "0", modPath, wwisePath });

            Assert.AreEqual(0, result);
            Assert.IsFalse(File.Exists(Path.Combine(bnksPath, @"pc\soundmetadata.bin.bak1")));
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/LinearBackups/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "AppTests/LinearBackups/Wwise/Default")]
        public void TestBackupArgumentLinearBackups()
        {
            var modPath = @"AppTests\LinearBackups\Mod\Default";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"AppTests\LinearBackups\Wwise\Default\ModsWwise";

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
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "AppTests/RotateBackups/Wwise/Default")]
        public void TestBackupArgumentRotateBackups()
        {
            var modPath = @"AppTests\RotateBackups\Mod\Default";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"AppTests\RotateBackups\Wwise\Default\ModsWwise";

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
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "AppTests/TestDefaultOutput/Wwise/Default")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestDefaultOutput.txt", "Comparisons/AppTests")]
        public void TestDefaultOutput()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"AppTests\TestDefaultOutput\Mod\Default";
            var wwisePath = @"AppTests\TestDefaultOutput\Wwise\Default\ModsWwise";

            int result = App.Main(new string[] { modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestDefaultOutput.txt")
                .Replace("{AssemblyVersion}", typeof(App).Assembly.GetName().Version.ToString())
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringAssertion.AreEqualNormalized(expected, writer.ToString());
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/TestVerboseOutput/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "AppTests/TestVerboseOutput/Wwise/Default")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestVerboseOutput.txt", "Comparisons/AppTests")]
        public void TestVerboseOutput()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"AppTests\TestVerboseOutput\Mod\Default";
            var wwisePath = @"AppTests\TestVerboseOutput\Wwise\Default\ModsWwise";

            int result = App.Main(new string[] { "-v", modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestVerboseOutput.txt")
                .Replace("{AssemblyVersion}", typeof(App).Assembly.GetName().Version.ToString())
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringAssertion.AreEqualNormalized(expected, writer.ToString());
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/TestDebugOutput/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "AppTests/TestDebugOutput/Wwise/Default")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestDebugOutput.txt", "Comparisons/AppTests")]
        public void TestDebugOutput()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"AppTests\TestDebugOutput\Mod\Default";
            var wwisePath = @"AppTests\TestDebugOutput\Wwise\Default\ModsWwise";

            int result = App.Main(new string[] { "-d", modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestDebugOutput.txt")
                .Replace("{AssemblyVersion}", typeof(App).Assembly.GetName().Version.ToString())
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringAssertion.AreEqualNormalized(expected, writer.ToString());
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/TestWithSoundContainerFix/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "AppTests/TestWithSoundContainerFix/Wwise/Default")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestWithSoundContainerFixOutput.txt", "Comparisons/AppTests")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestWithSoundContainerFixOriginalMetadata.json", "Comparisons/AppTests")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestWithSoundContainerFixWrittenMetadata.json", "Comparisons/AppTests")]
        public void TestWithSoundContainerFix()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"AppTests\TestWithSoundContainerFix\Mod\Default";
            var wwisePath = @"AppTests\TestWithSoundContainerFix\Wwise\Default\ModsWwise";

            int result = App.Main(new string[] { modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestWithSoundContainerFixOutput.txt")
                .Replace("{AssemblyVersion}", typeof(App).Assembly.GetName().Version.ToString())
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringAssertion.AreEqualNormalized(expected, writer.ToString());

            var metadata = SoundMetadata.FromFile(Path.Combine(modPath, @"base\sound\soundbanks\pc\soundmetadata.bin.bak1"));
            var writtenMetadata = SoundMetadata.FromFile(Path.Combine(modPath, @"base\sound\soundbanks\pc\soundmetadata.bin"));

            // Path parts capitalization is different in the SoundbanksInfo.xml file
            // and the soundmetadata.bin file... very funny.

            // TODO Shouldn't be a problem at runtime, as they appear to be treated as
            // case insensitive, but this needs confirmation!

            // TODO Since game/idstudio development has come to a hold (end?) now, maybe
            // this should instead just read the defaults from the vanilla game
            // soundmetadata.bin file, or maybe even maintain it decoded here in the app.

            metadata.PathParts = metadata.PathParts.Select(x => x.ToLower()).ToList();
            metadata.PathParts.Sort();
            metadata.SoundEvents = metadata.SoundEvents
                .Select((x) => {
                    x.PathParts = x.PathParts.Select(y => y.ToLower()).ToList();

                    return x;
                })
                .ToList();

            writtenMetadata.PathParts = writtenMetadata.PathParts.Select(x => x.ToLower()).ToList();
            writtenMetadata.PathParts.Sort();
            writtenMetadata.SoundEvents = metadata.SoundEvents
                .Select((x) => {
                    x.PathParts = x.PathParts.Select(y => y.ToLower()).ToList();

                    return x;
                })
                .ToList();

            string json = Json.SerializeObject(metadata);
            string writtenJson = Json.SerializeObject(writtenMetadata);

            Assert.AreEqual(File.ReadAllText(@"Comparisons\AppTests\TestWithSoundContainerFixOriginalMetadata.json"), json);
            Assert.AreEqual(File.ReadAllText(@"Comparisons\AppTests\TestWithSoundContainerFixWrittenMetadata.json"), writtenJson);
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "AppTests/TestWithoutSoundContainerFix/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "AppTests/TestWithoutSoundContainerFix/Wwise/Default")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestWithoutSoundContainerFixOutput.txt", "Comparisons/AppTests")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestWithoutSoundContainerFixOriginalMetadata.json", "Comparisons/AppTests")]
        [DeploymentItem("TestData/Comparisons/AppTests/TestWithoutSoundContainerFixWrittenMetadata.json", "Comparisons/AppTests")]
        public void TestWithoutSoundContainerFix()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var modPath = @"AppTests\TestWithoutSoundContainerFix\Mod\Default";
            var wwisePath = @"AppTests\TestWithoutSoundContainerFix\Wwise\Default\ModsWwise";

            int result = App.Main(new string[] { "-no-snd-fix", modPath, wwisePath });

            Assert.AreEqual(0, result);

            var expected =
                File
                .ReadAllText(@"Comparisons\AppTests\TestWithoutSoundContainerFixOutput.txt")
                .Replace("{AssemblyVersion}", typeof(App).Assembly.GetName().Version.ToString())
                .Replace("{TestDeploymentDir}", TestContext.TestDeploymentDir);

            StringAssertion.AreEqualNormalized(expected, writer.ToString());

            var metadata = SoundMetadata.FromFile(Path.Combine(modPath, @"base\sound\soundbanks\pc\soundmetadata.bin.bak1"));
            var writtenMetadata = SoundMetadata.FromFile(Path.Combine(modPath, @"base\sound\soundbanks\pc\soundmetadata.bin"));

            metadata.PathParts = metadata.PathParts.Select(x => x.ToLower()).ToList();
            metadata.PathParts.Sort();
            metadata.SoundEvents = metadata.SoundEvents
                .Select((x) => {
                    x.PathParts = x.PathParts.Select(y => y.ToLower()).ToList();

                    return x;
                })
                .ToList();

            writtenMetadata.PathParts = writtenMetadata.PathParts.Select(x => x.ToLower()).ToList();
            writtenMetadata.PathParts.Sort();
            writtenMetadata.SoundEvents = metadata.SoundEvents
                .Select((x) => {
                    x.PathParts = x.PathParts.Select(y => y.ToLower()).ToList();

                    return x;
                })
                .ToList();

            string json = Json.SerializeObject(metadata);
            string writtenJson = Json.SerializeObject(writtenMetadata);

            Assert.AreEqual(File.ReadAllText(@"Comparisons\AppTests\TestWithoutSoundContainerFixOriginalMetadata.json"), json);
            Assert.AreEqual(File.ReadAllText(@"Comparisons\AppTests\TestWithoutSoundContainerFixWrittenMetadata.json"), writtenJson);
        }
    }
}
