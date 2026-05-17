using EternalSoundMetadataPatcher.ConsoleIO;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EternalSoundMetadataPatcher.Metadata
{
    public class SoundMetadata
    {
        public List<string> PathParts = new List<string>();
        public List<SoundEvent> SoundEvents = new List<SoundEvent>();

        private byte[] PrecedingData;
        private byte[] TrailingData;

        private SoundMetadata() { }

        /// <summary>
        /// Writes the current sound metadata to a file.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        public void WriteTo(string path)
        {
            // TODO should the writer be responsible for ordering the section entries like events?

            Output.Information(string.Format("Writing soundmetadata file `{0}`", path));

            using (var stream = File.OpenWrite(path))
            {
                stream.SetLength(0);

                using (var writer = new BinaryWriter(stream, Encoding.Default, true))
                {
                    Output.Verbose(string.Format("Writing `{0}` bytes of preceding data", PrecedingData.Length));

                    writer.Write(PrecedingData);

                    Output.Verbose(string.Format("Writing `{0}` object path parts", PathParts.Count));

                    uint pathNodeSectionLength = (uint)PathParts.Sum(x => x.Length + 1);
                    writer.Write(pathNodeSectionLength);

                    Dictionary<string, uint> pathPartOffsets = new Dictionary<string, uint>();
                    uint pathNodeOffset = 0;
                    foreach (string pathPart in PathParts)
                    {
                        writer.Write(Encoding.UTF8.GetBytes(pathPart));
                        writer.Write((byte)0);

                        pathPartOffsets[pathPart.ToLowerInvariant()] = pathNodeOffset;
                        pathNodeOffset += (uint)pathPart.Length + 1;
                    }

                    Output.Verbose(string.Format("Writing `{0}` sound events", SoundEvents.Count));

                    writer.Write((uint)SoundEvents.Count);
                    foreach (SoundEvent soundEvent in SoundEvents)
                    {
                        writer.Write(soundEvent.Id);
                        byte[] eventNameBytes = Encoding.UTF8.GetBytes(soundEvent.Name);
                        uint eventNameLength = (uint)eventNameBytes.Length;
                        writer.Write(eventNameLength);
                        writer.Write(eventNameBytes);
                        writer.Write(soundEvent.Attenuation);
                        writer.Write(soundEvent.Is2D);
                        writer.Write(soundEvent.IsLooping);
                        writer.Write(soundEvent.IsExternalSource);
                        writer.Write(soundEvent.Duration);
                        writer.Write(soundEvent.StopEventId);
                        writer.Write((uint)soundEvent.SoundIds.Count);
                        foreach (uint soundId in soundEvent.SoundIds)
                        {
                            writer.Write(soundId);
                        }
                        writer.Write((uint)soundEvent.SoundbankIds.Count);
                        foreach (uint soundBankId in soundEvent.SoundbankIds)
                        {
                            writer.Write(soundBankId);
                        }
                        writer.Write((uint)soundEvent.PathParts.Count);
                        foreach (string pathPart in soundEvent.PathParts)
                        {
                            writer.Write(pathPartOffsets[pathPart.ToLowerInvariant()]);
                        }
                    }

                    Output.Verbose(string.Format("Writing `{0}` bytes of trailing data", TrailingData.Length));

                    writer.Write(TrailingData);
                }

                Output.Verbose(string.Format("Wrote `{0}` bytes", stream.Length));
            }
        }

        /// <summary>
        /// Reads the sound metadata from a soundmetadata.bin file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A SoundMetadata instance holding the extracted sound metadata.</returns>
        public static SoundMetadata FromFile(string path)
        {
            Output.Information(string.Format("Processing soundmetadata file `{0}`", path));

            byte[] precedingData;
            byte[] trailingData;

            List<string> pathParts = new List<string>();
            List<SoundEvent> soundEvents = new List<SoundEvent>();

            using (var stream = File.OpenRead(path))
            {
                using (var reader = new BinaryReader(stream, Encoding.Default, true))
                {
                    int version = reader.ReadInt32();
                    if (version != 24)
                    {
                        throw new UnsupportedMetadataVersionException();
                    }

                    // skip pck section
                    Output.Verbose(string.Format("Skipping PCK section at `{0}`", stream.Position));

                    int pckCount = reader.ReadInt32();
                    for (var i = 0; i < pckCount; i++)
                    {
                        uint pckNameLength = reader.ReadUInt32();
                        stream.Seek(pckNameLength, SeekOrigin.Current);
                        stream.Seek(4, SeekOrigin.Current); // soundbank id ?
                    }

                    // skip snd section
                    Output.Verbose(string.Format("Skipping sound container section at `{0}`", stream.Position));

                    int sndCount = reader.ReadInt32();
                    for (var i = 0; i < sndCount; i++)
                    {
                        uint sndNameLength = reader.ReadUInt32();
                        stream.Seek(sndNameLength, SeekOrigin.Current);

                        uint sndContainerCount = reader.ReadUInt32();
                        for (var j = 0; j < sndContainerCount; j++)
                        {
                            stream.Seek(4, SeekOrigin.Current); // id
                            uint maskLength = reader.ReadUInt32();
                            stream.Seek(maskLength, SeekOrigin.Current); // file validity mask
                        }
                    }

                    // skip bnk section
                    Output.Verbose(string.Format("Skipping BNK section at `{0}`", stream.Position));

                    int bnkCount = reader.ReadInt32();
                    for (var i = 0; i < bnkCount; i++)
                    {
                        uint bnkNameLength = reader.ReadUInt32();
                        stream.Seek(bnkNameLength, SeekOrigin.Current);
                        stream.Seek(4, SeekOrigin.Current); // id
                    }

                    // skip aux bus section
                    Output.Verbose(string.Format("Skipping Aux Bus section at `{0}`", stream.Position));

                    uint busCount = reader.ReadUInt32();
                    for (var i = 0; i < busCount; i++)
                    {
                        stream.Seek(4, SeekOrigin.Current); // id
                        uint busNameLength = reader.ReadUInt32();
                        stream.Seek(busNameLength, SeekOrigin.Current);
                    }

                    // skip rtpc section
                    Output.Verbose(string.Format("Skipping RTPC section at `{0}`", stream.Position));

                    uint rtpcCount = reader.ReadUInt32();
                    for (var i = 0; i < rtpcCount; i++)
                    {
                        stream.Seek(4, SeekOrigin.Current); // id
                        uint rtpcNameLength = reader.ReadUInt32();
                        stream.Seek(rtpcNameLength, SeekOrigin.Current);
                    }

                    // skip switch section
                    Output.Verbose(string.Format("Skipping switch group section at `{0}`", stream.Position));

                    uint switchGroupCount = reader.ReadUInt32();
                    for (var i = 0; i < switchGroupCount; i++)
                    {
                        stream.Seek(4, SeekOrigin.Current); // id
                        uint switchGroupNameLength = reader.ReadUInt32();
                        stream.Seek(switchGroupNameLength, SeekOrigin.Current);

                        uint switchCount = reader.ReadUInt32();
                        for (var j = 0; j < switchCount; j++)
                        {
                            stream.Seek(4, SeekOrigin.Current); // id
                            uint switchNameLength = reader.ReadUInt32();
                            stream.Seek(switchNameLength, SeekOrigin.Current);
                        }
                    }

                    // skip state section
                    Output.Verbose(string.Format("Skipping state section at `{0}`", stream.Position));

                    int stateGroupCount = reader.ReadInt32();
                    for (var i = 0; i < stateGroupCount; i++)
                    {
                        stream.Seek(4, SeekOrigin.Current); // id
                        uint stateGroupNameLength = reader.ReadUInt32();
                        stream.Seek(stateGroupNameLength, SeekOrigin.Current);

                        uint stateCount = reader.ReadUInt32();
                        for (var j = 0; j < stateCount; j++)
                        {
                            stream.Seek(4, SeekOrigin.Current); // id
                            uint stateNameLength = reader.ReadUInt32();
                            stream.Seek(stateNameLength, SeekOrigin.Current);
                        }
                    }

                    // object path parts
                    Output.Verbose(string.Format("Processing object path parts section at `{0}`", stream.Position));

                    long pathNodeSectionOffset = stream.Position;
                    uint pathNodeSectionLength = reader.ReadUInt32();

                    Dictionary<uint, string> pathNodes = new Dictionary<uint, string>();
                    List<byte> pathPartChars = new List<byte>();
                    uint pathNodeOffset = 0;
                    for (var i = 0; i < pathNodeSectionLength; i++)
                    {
                        byte character = reader.ReadByte();
                        if (character == 0)
                        {
                            string pathPartName = Encoding.UTF8.GetString(pathPartChars.ToArray());
                            pathNodes[pathNodeOffset] = pathPartName;
                            pathParts.Add(pathPartName);

                            Output.Debug(string.Format("Processing `[ObjectPathPart Offset='{0}' Name='{0}']`", pathNodeOffset, pathPartName));

                            pathNodeOffset += (uint)pathPartChars.Count + 1;
                            pathPartChars.Clear();

                            continue;
                        }
                        pathPartChars.Add(character);
                    }

                    Output.Verbose(string.Format("Extracted `{0}` object path parts", pathParts.Count));

                    // events
                    Output.Verbose(string.Format("Processing events section at `{0}`", stream.Position));

                    uint eventCount = reader.ReadUInt32();
                    Output.Verbose(string.Format("Expecting `{0}` events", eventCount));

                    for (var i = 0; i < eventCount; i++)
                    {
                        uint eventId = reader.ReadUInt32();
                        uint eventNameLength = reader.ReadUInt32();
                        byte[] nameBytes = new byte[eventNameLength];
                        reader.Read(nameBytes, 0, (int)eventNameLength);
                        string eventName = Encoding.UTF8.GetString(nameBytes);

                        Output.Debug(string.Format("Processing `[Event Name='{0}' Id='{1}']`", eventName, eventId));

                        float attenuation = reader.ReadSingle();
                        bool is2D = reader.ReadBoolean();
                        bool isLooping = reader.ReadBoolean();
                        bool isExternalSource = reader.ReadBoolean();
                        uint duration = reader.ReadUInt32();
                        uint stopEventId = reader.ReadUInt32();

                        List<uint> soundIds = new List<uint>();
                        uint soundIdsCount = reader.ReadUInt32();
                        for (var j = 0; j < soundIdsCount; j++)
                        {
                            soundIds.Add(reader.ReadUInt32());
                        }

                        List<uint> soundbankIds = new List<uint>();
                        uint soundBankIdsCount = reader.ReadUInt32();
                        for (var j = 0; j < soundBankIdsCount; j++)
                        {
                            soundbankIds.Add(reader.ReadUInt32());
                        }

                        List<string> objectPathParts = new List<string>();
                        uint objectPathPartOffsetsCount = reader.ReadUInt32();
                        for (var j = 0; j < objectPathPartOffsetsCount; j++)
                        {
                            uint offset = reader.ReadUInt32();
                            objectPathParts.Add(pathNodes[offset]);
                        }

                        soundEvents.Add(
                            new SoundEvent
                            {
                                Id = eventId,
                                Name = eventName,
                                Attenuation = attenuation,
                                Is2D = is2D,
                                IsLooping = isLooping,
                                IsExternalSource = isExternalSource,
                                Duration = duration,
                                StopEventId = stopEventId,
                                SoundIds = soundIds,
                                SoundbankIds = soundbankIds,
                                PathParts = objectPathParts
                            }
                        );
                    }

                    Output.Verbose(string.Format("Extracted `{0}` events", soundEvents.Count));

                    long eventPrefetchSectionOffset = stream.Position;

                    precedingData = new byte[pathNodeSectionOffset];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(precedingData, 0, (int)pathNodeSectionOffset);

                    stream.Seek(eventPrefetchSectionOffset, SeekOrigin.Begin);

                    // skip event prefetch section
                    Output.Verbose(string.Format("Skipping event prefetch section at `{0}`", stream.Position));

                    // TODO How is prefetching used in the game anyways? Do we even want/need to support this for custom sounds?

                    var remainingLength = (int)(stream.Length - stream.Position);
                    trailingData = new byte[remainingLength];
                    stream.Read(trailingData, 0, remainingLength);
                }
            }

            return new SoundMetadata
            {
                PathParts = pathParts,
                SoundEvents = soundEvents,
                PrecedingData = precedingData,
                TrailingData = trailingData,
            };
        }
    }
}