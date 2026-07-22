using EternalSoundMetadataPatcher.Backups;
using EternalSoundMetadataPatcher.Patching;
using EternalSoundMetadataPatcher.Test.TestTools.Assertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class PatcherTests
    {
        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "PatcherTests/TestNonExistentModSoundbanksInfoFile/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "PatcherTests/TestNonExistentModSoundbanksInfoFile/Wwise/Default")]
        public void TestNonExistentModSoundbanksInfoFile()
        {
            var modPath = @"PatcherTests\TestNonExistentModSoundbanksInfoFile\Mod\Default";
            var wwisePath = @"PatcherTests\TestNonExistentModSoundbanksInfoFile\Wwise\Default\ModsWwise";

            File.Delete(Path.Combine(modPath, @"base\sound\soundbanks\SoundbanksInfo.xml"));

            ExceptionAssertion.ThrowsWithMessage<FileNotFoundException>(
                @"Missing mod soundbanks info file `PatcherTests\TestNonExistentModSoundbanksInfoFile\Mod\Default\base\sound\soundbanks\SoundbanksInfo.xml`.",
                () => Patcher.Patch(modPath, wwisePath, null)
            );
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "PatcherTests/TestNonExistentModSoundMetadataFile/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "PatcherTests/TestNonExistentModSoundMetadataFile/Wwise/Default")]
        public void TestNonExistentModSoundMetadataFile()
        {
            var modPath = @"PatcherTests\TestNonExistentModSoundMetadataFile\Mod\Default";
            var wwisePath = @"PatcherTests\TestNonExistentModSoundMetadataFile\Wwise\Default\ModsWwise";

            File.Delete(Path.Combine(modPath, @"base\sound\soundbanks\pc\soundmetadata.bin"));

            ExceptionAssertion.ThrowsWithMessage<FileNotFoundException>(
                @"Missing mod sound metadata file `PatcherTests\TestNonExistentModSoundMetadataFile\Mod\Default\base\sound\soundbanks\pc\soundmetadata.bin`.",
                () => Patcher.Patch(modPath, wwisePath, null)
            );
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "PatcherTests/TestNonExistentWwiseDirectory/Mod/Default")]
        public void TestNonExistentWwiseDirectory()
        {
            var modPath = @"PatcherTests\TestNonExistentWwiseDirectory\Mod\Default";
            var wwisePath = @"PatcherTests\TestNonExistentWwiseDirectory\Wwise\Default\ModsWwise";

            ExceptionAssertion.ThrowsWithMessage<FileNotFoundException>(
                @"Missing soundbanks info file `PatcherTests\TestNonExistentWwiseDirectory\Wwise\Default\ModsWwise\GeneratedSoundBanks\Windows\SoundbanksInfo.xml`. " +
                "Make sure that you have packaged your Wwise project!",
                () => Patcher.Patch(modPath, wwisePath, null)
            );
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "PatcherTests/TestNonExistentWwiseSoundbanksInfoFile/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "PatcherTests/Wwise/Default")]
        public void TestNonExistentWwiseSoundbanksInfoFile()
        {
            var modPath = @"PatcherTests\TestNonExistentWwiseSoundbanksInfoFile\Mod\Default";
            var wwisePath = @"PatcherTests\Wwise\Default\ModsWwise";

            File.Delete(Path.Combine(wwisePath, @"GeneratedSoundBanks\Windows\SoundbanksInfo.xml"));

            ExceptionAssertion.ThrowsWithMessage<FileNotFoundException>(
                @"Missing soundbanks info file `PatcherTests\Wwise\Default\ModsWwise\GeneratedSoundBanks\Windows\SoundbanksInfo.xml`. " +
                "Make sure that you have packaged your Wwise project!",
                () => Patcher.Patch(modPath, wwisePath, null)
            );
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "PatcherTests/TestNonExistentWwiseAttenuationsFile/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "PatcherTests/Wwise/Default")]
        public void TestNonExistentWwiseAttenuationsFile()
        {
            var modPath = @"PatcherTests\TestNonExistentWwiseAttenuationsFile\Mod\Default";
            var wwisePath = @"PatcherTests\Wwise\Default\ModsWwise";

            File.Delete(Path.Combine(wwisePath, @"Attenuations\Default Work Unit.wwu"));

            ExceptionAssertion.ThrowsWithMessage<FileNotFoundException>(
                @"Missing attenuations file `PatcherTests\Wwise\Default\ModsWwise\Attenuations\Default Work Unit.wwu`.",
                () => Patcher.Patch(modPath, wwisePath, null)
            );
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "PatcherTests/TestNonExistentWwiseAudioObjectsFile/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "PatcherTests/TestNonExistentWwiseAudioObjectsFile/Wwise/Default")]
        public void TestNonExistentWwiseAudioObjectsFile()
        {
            var modPath = @"PatcherTests\TestNonExistentWwiseAudioObjectsFile\Mod\Default";
            var wwisePath = @"PatcherTests\TestNonExistentWwiseAudioObjectsFile\Wwise\Default\ModsWwise";

            File.Delete(Path.Combine(wwisePath, @"Actor-Mixer Hierarchy\ghost_mod.wwu"));

            ExceptionAssertion.ThrowsWithMessage<FileNotFoundException>(
                @"Missing audio objects file `PatcherTests\TestNonExistentWwiseAudioObjectsFile\Wwise\Default\ModsWwise\Actor-Mixer Hierarchy\ghost_mod.wwu`. " +
                "Make sure that you are using the correct mod sounds project!",
                () => Patcher.Patch(modPath, wwisePath, null)
            );
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "PatcherTests/TestNonExistentWwiseEventsFile/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "PatcherTests/TestNonExistentWwiseEventsFile/Wwise/Default")]
        public void TestNonExistentWwiseEventsFile()
        {
            var modPath = @"PatcherTests\TestNonExistentWwiseEventsFile\Mod\Default";
            var wwisePath = @"PatcherTests\TestNonExistentWwiseEventsFile\Wwise\Default\ModsWwise";

            File.Delete(Path.Combine(wwisePath, @"Events\mod_events.wwu"));

            ExceptionAssertion.ThrowsWithMessage<FileNotFoundException>(
                @"Missing events file `PatcherTests\TestNonExistentWwiseEventsFile\Wwise\Default\ModsWwise\Events\mod_events.wwu`. " +
                "Make sure that you are using the correct mod sounds project!",
                () => Patcher.Patch(modPath, wwisePath, null)
            );
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "PatcherTests/TestNonExistentWwiseExternalSourcesFile/Mod/Default")]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "PatcherTests/TestNonExistentWwiseExternalSourcesFile/Wwise/Default")]
        public void TestNonExistentWwiseExternalSourcesFile()
        {
            var modPath = @"PatcherTests\TestNonExistentWwiseExternalSourcesFile\Mod\Default";
            var wwisePath = @"PatcherTests\TestNonExistentWwiseExternalSourcesFile\Wwise\Default\ModsWwise";

            File.Delete(Path.Combine(wwisePath, @"..\ModWavs\test.wsources"));

            ExceptionAssertion.ThrowsWithMessage<FileNotFoundException>(
                @"Missing external sources file `PatcherTests\TestNonExistentWwiseExternalSourcesFile\Wwise\Default\ModsWwise\..\ModWavs\test.wsources`. " +
                "Make sure that you are using the correct mod sounds project!",
                () => Patcher.Patch(modPath, wwisePath, null)
            );
        }
    }
}
