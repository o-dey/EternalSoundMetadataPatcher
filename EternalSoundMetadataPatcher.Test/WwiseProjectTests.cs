using EternalSoundMetadataPatcher.Wwise;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class WwiseProjectTests
    {
        [TestMethod]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "Default")]
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

            var expected = new List<string> {
                "Default Work Unit",
                "mod_events",
                "ghost_ambience_mod",
                "ghost_fodder_mod",
                "ghost_generic_mod",
                "ghost_weapon_mod",
            };
            Assert.IsTrue(project.SoundbanksInfo.ObjectPathParts.SequenceEqual(expected));
        }

        [TestMethod]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "Custom")]
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

            // TODO test all the stuff

            var expectedAudioFiles = new List<string> {
                @"..\ModWavs\generic\generic1.wav",
                @"..\ModWavs\generic\generic2.wav",
                @"..\ModWavs\generic\generic3.wav",
                @"Custom\ModsWwise\originals\SFX\ambient_01.wav",
                @"Custom\ModsWwise\originals\SFX\ambient_02.wav",
                @"Custom\ModsWwise\originals\SFX\vo_4TH_PRIEST__Here_to_lecture_us.wav",
                @"Custom\ModsWwise\originals\SFX\vo_4TH_PRIEST__Look_who_it_is.wav",
            };
            Assert.IsTrue(project.AudioFiles.Select(x => x.Path).SequenceEqual(expectedAudioFiles));

            var expected = new List<string> {
                "Default Work Unit",
                "mod_events",
                "ghost_generic_mod",
                "ghost_vo_mod",
            };
            Assert.IsTrue(project.SoundbanksInfo.ObjectPathParts.SequenceEqual(expected));
        }
    }
}
