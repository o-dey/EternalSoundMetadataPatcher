using EternalSoundMetadataPatcher.Test.TestTools.Serialization;
using EternalSoundMetadataPatcher.Wwise.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class SoundbanksInfoTests
    {
        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "SoundbanksInfoTests/Mod/Default")]
        [DeploymentItem("TestData/Comparisons/SoundbanksInfoTests/TestBankFilterNoMatches.json", "Comparisons/SoundbanksInfoTests")]
        public void TestBankFilterNoMatches()
        {
            var info = SoundbanksInfo.FromFile(@"SoundbanksInfoTests\Mod\Default\base\sound\soundbanks\SoundbanksInfo.xml", x => false);

            Assert.AreEqual(0, info.SoundbankIds.Count);
            Assert.AreEqual(0, info.EventIds.Count);
            Assert.AreEqual(0, info.ExternalSources.Count);
            Assert.AreEqual(0, info.MediaFiles.Count);
            Assert.AreEqual(0, info.ObjectPathParts.Count);

            string json = Json.SerializeObject(info);
            Assert.AreEqual(File.ReadAllText(@"Comparisons\SoundbanksInfoTests\TestBankFilterNoMatches.json"), json);
        }

        [TestMethod]
        [DeploymentItem("TestData/Mod/Default", "SoundbanksInfoTests/Mod/Default")]
        [DeploymentItem("TestData/Comparisons/SoundbanksInfoTests/TestOriginalAllBanks.json", "Comparisons/SoundbanksInfoTests")]
        public void TestOriginalAllBanks()
        {
            var info = SoundbanksInfo.FromFile(@"SoundbanksInfoTests\Mod\Default\base\sound\soundbanks\SoundbanksInfo.xml");

            Assert.AreEqual(7, info.SoundbankIds.Count);
            Assert.AreEqual(5895, info.EventIds.Count);
            Assert.AreEqual(113, info.ExternalSources.Count);
            Assert.AreEqual(18106, info.MediaFiles.Count);
            Assert.AreEqual(461, info.ObjectPathParts.Count);

            string json = Json.SerializeObject(info);
            Assert.AreEqual(File.ReadAllText(@"Comparisons\SoundbanksInfoTests\TestOriginalAllBanks.json"), json);
        }

        [TestMethod]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_modded.xml")]
        [DeploymentItem("TestData/Comparisons/SoundbanksInfoTests/TestModdedAllBanks.json", "Comparisons/SoundbanksInfoTests")]
        public void TestModdedAllBanks()
        {
            var info = SoundbanksInfo.FromFile("SoundbanksInfo_modded.xml");

            Assert.AreEqual(3, info.SoundbankIds.Count);
            Assert.AreEqual(15, info.EventIds.Count);
            Assert.AreEqual(5, info.ExternalSources.Count);
            Assert.AreEqual(10, info.MediaFiles.Count);
            Assert.AreEqual(4, info.ObjectPathParts.Count);

            string json = Json.SerializeObject(info);
            Assert.AreEqual(File.ReadAllText(@"Comparisons\SoundbanksInfoTests\TestModdedAllBanks.json"), json);
        }
    }
}
