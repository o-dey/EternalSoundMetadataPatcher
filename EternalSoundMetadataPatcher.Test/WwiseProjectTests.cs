using EternalSoundMetadataPatcher.Test.TestTools.Serialization;
using EternalSoundMetadataPatcher.Wwise;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class WwiseProjectTests
    {
        [TestMethod]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "Default")]
        [DeploymentItem("TestData/Comparisons/WwiseProjectTests/TestDefaultTemplate.json", "Comparisons/WwiseProjectTests")]
        public void TestDefaultTemplate()
        {
            WwiseProject project = WwiseProject.FromDirectory(@"Default\ModsWwise");

            Assert.AreEqual(133, project.AudioFiles.Count);
            Assert.AreEqual(144, project.AudioObjects.Count);
            Assert.AreEqual(71, project.EventObjects.Count);

            Assert.AreEqual(1, project.SoundbanksInfo.SoundbankIds.Count);
            Assert.AreEqual(71, project.SoundbanksInfo.EventIds.Count);
            Assert.AreEqual(113, project.SoundbanksInfo.ExternalSources.Count);
            Assert.AreEqual(0, project.SoundbanksInfo.MediaFiles.Count);
            Assert.AreEqual(6, project.SoundbanksInfo.ObjectPathParts.Count);

            string json = Json.SerializeObject(project);
            Assert.AreEqual(File.ReadAllText(@"Comparisons\WwiseProjectTests\TestDefaultTemplate.json"), json);
        }

        [TestMethod]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "Custom")]
        [DeploymentItem("TestData/Comparisons/WwiseProjectTests/TestCustom.json", "Comparisons/WwiseProjectTests")]
        public void TestCustom()
        {
            WwiseProject project = WwiseProject.FromDirectory(@"Custom\ModsWwise");

            Assert.AreEqual(7, project.AudioFiles.Count);
            Assert.AreEqual(7, project.AudioObjects.Count);
            Assert.AreEqual(6, project.EventObjects.Count);

            Assert.AreEqual(1, project.SoundbanksInfo.SoundbankIds.Count);
            Assert.AreEqual(6, project.SoundbanksInfo.EventIds.Count);
            Assert.AreEqual(3, project.SoundbanksInfo.ExternalSources.Count);
            Assert.AreEqual(2, project.SoundbanksInfo.MediaFiles.Count);
            Assert.AreEqual(4, project.SoundbanksInfo.ObjectPathParts.Count);

            string json = Json.SerializeObject(project);
            Assert.AreEqual(File.ReadAllText(@"Comparisons\WwiseProjectTests\TestCustom.json"), json);
        }
    }
}
