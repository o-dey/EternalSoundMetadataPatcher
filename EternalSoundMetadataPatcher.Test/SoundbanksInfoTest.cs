using EternalSoundMetadataPatcher.Wwise.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class SoundbanksInfoTest
    {
        [TestMethod]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_orig.xml")]
        public void TestBankFilterNoMatches()
        {
            var info = SoundbanksInfo.FromFile("SoundbanksInfo_orig.xml", x => false);

            Assert.AreEqual(0, info.SoundbankIds.Count);
            Assert.AreEqual(0, info.EventIds.Count);
            Assert.AreEqual(0, info.ExternalSources.Count);
            Assert.AreEqual(0, info.MediaFiles.Count);
            Assert.AreEqual(0, info.ObjectPathParts.Count);
        }

        [TestMethod]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_orig.xml")]
        public void TestOriginalAllBanks()
        {
            var info = SoundbanksInfo.FromFile("SoundbanksInfo_orig.xml");

            Assert.AreEqual(7, info.SoundbankIds.Count);
            Assert.AreEqual(5895, info.EventIds.Count);
            Assert.AreEqual(113, info.ExternalSources.Count);
            Assert.AreEqual(18106, info.MediaFiles.Count);
            Assert.AreEqual(461, info.ObjectPathParts.Count);
        }

        [TestMethod]
        [DeploymentItem("TestData/Wwise/Generated/SoundbanksInfo_modded.xml")]
        public void TestModdedAllBanks()
        {
            var info = SoundbanksInfo.FromFile("SoundbanksInfo_modded.xml");

            Assert.AreEqual(3, info.SoundbankIds.Count);
            Assert.AreEqual(15, info.EventIds.Count);
            Assert.AreEqual(5, info.ExternalSources.Count);
            Assert.AreEqual(10, info.MediaFiles.Count);
            Assert.AreEqual(4, info.ObjectPathParts.Count);
        }
    }
}
