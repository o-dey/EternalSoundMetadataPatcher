using EternalSoundMetadataPatcher.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class SoundMetadataTests
    {
        private string GetFileHash(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                }
            }
        }

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
        public void TestReadOriginal()
        {
            var metadata = SoundMetadata.FromFile("soundmetadata_orig.bin");

            Assert.AreEqual(461, metadata.PathParts.Count);
            Assert.AreEqual(5897, metadata.SoundEvents.Count);
        }

        [TestMethod]
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin")]
        public void TestWriteWithoutChanges()
        {
            var metadata = SoundMetadata.FromFile("soundmetadata_orig.bin");
            metadata.WriteTo("soundmetadata_new.bin");

            var origHash = GetFileHash("soundmetadata_orig.bin");
            var newHash = GetFileHash("soundmetadata_new.bin");
            Assert.AreEqual(origHash, newHash);
        }

        [TestMethod]
        [DeploymentItem("TestData/Metadata/soundmetadata_orig.bin")]
        public void TestWrite()
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

            metadata.WriteTo("soundmetadata_new.bin");

            Assert.AreEqual("7B55B50C832686C47761B73D95FFDC4E", GetFileHash("soundmetadata_new.bin"));
        }
    }
}
