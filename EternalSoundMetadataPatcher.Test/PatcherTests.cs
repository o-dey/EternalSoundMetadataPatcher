using EternalSoundMetadataPatcher.Backups;
using EternalSoundMetadataPatcher.Patching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class PatcherTests
    {
        [TestMethod]
        public void TestNonExistentModDirectory()
        {
            Assert.ThrowsException<DirectoryNotFoundException>(() => Patcher.Patch("foo", "bar", null));
        }

        [TestMethod]
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin", "non_existent_wwise/mod/base/sound/soundbanks/pc")]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_orig.xml", "non_existent_wwise/mod/base/sound/soundbanks")]
        public void TestNonExistentWwiseDirectory()
        {
            var modPath = @"non_existent_wwise\mod";
            var bnksPath = Path.Combine(modPath, @"base\sound\soundbanks");
            var wwisePath = @"non_existent_wwise\wwise\ModsWwise";

            File.Move(
                Path.Combine(bnksPath, @"pc\soundmetadata_orig.bin"),
                Path.Combine(bnksPath, @"pc\soundmetadata.bin")
            );
            File.Move(
                Path.Combine(bnksPath, "SoundbanksInfo_orig.xml"),
                Path.Combine(bnksPath, "SoundbanksInfo.xml")
            );

            Assert.ThrowsException<FileNotFoundException>(() => Patcher.Patch(modPath, wwisePath, null));
        }
    }
}
