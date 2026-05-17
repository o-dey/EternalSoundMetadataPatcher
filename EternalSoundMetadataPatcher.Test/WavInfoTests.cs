using EternalSoundMetadataPatcher.Audio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class WavInfoTests
    {
        [TestMethod]
        [DeploymentItem("TestData/Audio/Apps")]
        public void TestApps()
        {
            Assert.AreEqual(2.468375F, WavInfo.FromFile("audacity.wav").Duration, 0.000001);
            Assert.AreEqual(3.920021F, WavInfo.FromFile("avidemux.wav").Duration, 0.000001);
            Assert.AreEqual(2.468375F, WavInfo.FromFile("ffmpeg.wav").Duration, 0.000001);
            Assert.AreEqual(2.468375F, WavInfo.FromFile("wavelab.wav").Duration, 0.000001);
            Assert.AreEqual(1.2495F, WavInfo.FromFile("wwise.wav").Duration, 0.000001);
        }

        [TestMethod]
        [DeploymentItem("TestData/Audio/Resolutions")]
        public void TestResolutions()
        {
            Assert.AreEqual(2.468375F, WavInfo.FromFile("16bit.wav").Duration, 0.000001);
            Assert.AreEqual(2.468375F, WavInfo.FromFile("20bit.wav").Duration, 0.000001);
            Assert.AreEqual(2.468375F, WavInfo.FromFile("24bit.wav").Duration, 0.000001);
            Assert.AreEqual(2.468375F, WavInfo.FromFile("32bit.wav").Duration, 0.000001);
            Assert.AreEqual(2.468375F, WavInfo.FromFile("32bitf.wav").Duration, 0.000001);
            Assert.AreEqual(2.468375F, WavInfo.FromFile("64bitf.wav").Duration, 0.000001);
            Assert.AreEqual(2.468375F, WavInfo.FromFile("8bit.wav").Duration, 0.000001);
        }

        [TestMethod]
        [DeploymentItem("TestData/Audio/Specs/not_a_riff_file.wav")]
        public void TestSpecsNotARiffFile()
        {
            Assert.ThrowsException<MissingRiffMarkerException>(() => WavInfo.FromFile("not_a_riff_file.wav"));
        }

        [TestMethod]
        [DeploymentItem("TestData/Audio/Specs/not_a_wave_file.wav")]
        public void TestSpecsNotAWaveFile()
        {
            Assert.ThrowsException<MissingWaveMarkerException>(() => WavInfo.FromFile("not_a_wave_file.wav"));
        }

        [TestMethod]
        [DeploymentItem("TestData/Audio/Specs/no_fmt_chunk.wav")]
        public void TestSpecsMissingFmtChunk()
        {
            Assert.ThrowsException<MissingFmtChunkException>(() => WavInfo.FromFile("no_fmt_chunk.wav"));
        }

        [TestMethod]
        [DeploymentItem("TestData/Audio/Specs/no_data_chunk.wav")]
        public void TestSpecsMissingDataChunk()
        {
            Assert.ThrowsException<MissingDataChunkException>(() => WavInfo.FromFile("no_data_chunk.wav"));
        }
    }
}
