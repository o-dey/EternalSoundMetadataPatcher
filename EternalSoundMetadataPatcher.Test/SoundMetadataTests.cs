using EternalSoundMetadataPatcher.Metadata;
using EternalSoundMetadataPatcher.Test.TestTools.Hashing;
using EternalSoundMetadataPatcher.Test.TestTools.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class SoundMetadataTests
    {
        [TestMethod]
        [DeploymentItem("TestData/Metadata/soundmetadata_unsupported_version.bin")]
        public void TestUnsupportedVersion()
        {
            Assert.ThrowsException<UnsupportedMetadataVersionException>(
                () => SoundMetadata.FromFile("soundmetadata_unsupported_version.bin")
            );
        }

        [TestMethod]
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin")]
        [DeploymentItem("TestData/Comparisons/SoundMetadataTests/TestReadOriginal.json", "Comparisons/SoundMetadataTests")]
        public void TestReadOriginal()
        {
            var metadata = SoundMetadata.FromFile("soundmetadata_orig.bin");

            Assert.AreEqual(461, metadata.PathParts.Count);
            Assert.AreEqual(5897, metadata.SoundEvents.Count);

            string json = Json.SerializeObject(metadata);
            Assert.AreEqual(File.ReadAllText(@"Comparisons\SoundMetadataTests\TestReadOriginal.json"), json);
        }

        [TestMethod]
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin")]
        public void TestWriteWithoutChanges()
        {
            var metadata = SoundMetadata.FromFile("soundmetadata_orig.bin");
            metadata.WriteTo("soundmetadata_new.bin");

            var origHash = MD5.GetFileHash("soundmetadata_orig.bin");
            var newHash = MD5.GetFileHash("soundmetadata_new.bin");
            Assert.AreEqual(origHash, newHash);
        }

        [TestMethod]
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin")]
        [DeploymentItem("TestData/Comparisons/SoundMetadataTests/TestWriteWithChanges.json", "Comparisons/SoundMetadataTests")]
        public void TestWriteWithChanges()
        {
            var metadata = SoundMetadata.FromFile("soundmetadata_orig.bin");

            metadata.PathParts.Add("Foo");
            metadata.PathParts.Add("Bar");
            metadata.PathParts.Add("Baz");

            metadata.SoundEvents.Add(new SoundEvent {
                Id = 1234,
                Name = "Play_foo_bar_baz",
                Attenuation = 12,
                Is2D = true,
                IsLooping = false,
                IsExternalSource = false,
                Duration = 1000,
                StopEventId = 98765,
                SoundIds = new List<uint> { 1, 2, 3 },
                SoundbankIds = new List<uint> { 56789 },
                PathParts = new List<string> { "Foo", "Bar", "Baz" },
            });

            Assert.AreEqual(461 + 3, metadata.PathParts.Count);
            Assert.AreEqual(5897 + 1, metadata.SoundEvents.Count);

            string json = Json.SerializeObject(metadata);
            Assert.AreEqual(File.ReadAllText(@"Comparisons\SoundMetadataTests\TestWriteWithChanges.json"), json);

            metadata.WriteTo("soundmetadata_new.bin");

            var writtenMetadata = SoundMetadata.FromFile("soundmetadata_new.bin");

            json = Json.SerializeObject(writtenMetadata);
            Assert.AreEqual(File.ReadAllText(@"Comparisons\SoundMetadataTests\TestWriteWithChanges.json"), json);
        }
    }
}
