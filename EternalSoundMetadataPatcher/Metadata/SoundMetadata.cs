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
        public List<SoundContainer> SoundContainers = new List<SoundContainer>();
        public List<string> PathParts = new List<string>();
        public List<SoundEvent> SoundEvents = new List<SoundEvent>();

        private byte[] HeaderBuffer;
        private byte[] PckBuffer;
        private byte[] BnkBuffer;
        private byte[] BusBuffer;
        private byte[] RtpcBuffer;
        private byte[] SwitchGroupBuffer;
        private byte[] StateGroupBuffer;
        private byte[] EventPrefetchBuffer;

        private SoundMetadata() { }

        private static long bufferStartPosition;

        private static void BufferMarkStart(Stream stream)
        {
            bufferStartPosition = stream.Position;
        }

        private static byte[] BufferRead(Stream stream)
        {
            long bufferEndPosition = stream.Position;
            long bufferSize = bufferEndPosition - bufferStartPosition;
            byte[] buffer = new byte[bufferSize];

            stream.Seek(bufferStartPosition, SeekOrigin.Begin);
            stream.Read(buffer, 0, (int)bufferSize);

            return buffer;
        }

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
                    Output.Verbose(string.Format("Writing `{0}` bytes of header data", HeaderBuffer.Length));
                    writer.Write(HeaderBuffer);

                    Output.Verbose(string.Format("Writing `{0}` bytes of PCK data", PckBuffer.Length));
                    writer.Write(PckBuffer);

                    Output.Verbose(string.Format("Writing `{0}` sound containers", SoundContainers.Count));

                    writer.Write((uint)SoundContainers.Count);
                    foreach (SoundContainer soundContainer in SoundContainers)
                    {
                        byte[] containerNameBytes = Encoding.UTF8.GetBytes(soundContainer.Name);
                        uint containerNameLength = (uint)containerNameBytes.Length;
                        writer.Write(containerNameLength);
                        writer.Write(containerNameBytes);

                        writer.Write((uint)soundContainer.Parts.Count);
                        foreach (SoundContainerPart containerPart in soundContainer.Parts)
                        {
                            writer.Write(containerPart.Id);
                            writer.Write((uint)containerPart.ValidityMask.Count);
                            foreach (uint validityMask in containerPart.ValidityMask)
                            {
                                writer.Write(validityMask);
                            }
                        }
                    }

                    Output.Verbose(string.Format("Writing `{0}` bytes of BNK data", BnkBuffer.Length));
                    writer.Write(BnkBuffer);

                    Output.Verbose(string.Format("Writing `{0}` bytes of Aux Bus data", BusBuffer.Length));
                    writer.Write(BusBuffer);

                    Output.Verbose(string.Format("Writing `{0}` bytes of RTPC data", RtpcBuffer.Length));
                    writer.Write(RtpcBuffer);

                    Output.Verbose(string.Format("Writing `{0}` bytes of switch group data", SwitchGroupBuffer.Length));
                    writer.Write(SwitchGroupBuffer);

                    Output.Verbose(string.Format("Writing `{0}` bytes of state group data", StateGroupBuffer.Length));
                    writer.Write(StateGroupBuffer);

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

                    Output.Verbose(string.Format("Writing `{0}` bytes of event prefetch data", EventPrefetchBuffer.Length));
                    writer.Write(EventPrefetchBuffer);
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

            byte[] headerBuffer;
            byte[] pckBuffer;
            byte[] bnkBuffer;
            byte[] busBuffer;
            byte[] rtpcBuffer;
            byte[] switchGroupBuffer;
            byte[] stateGroupBuffer;
            byte[] eventPrefetchBuffer;

            List<SoundContainer> soundContainers = new List<SoundContainer>();
            List<string> pathParts = new List<string>();
            List<SoundEvent> soundEvents = new List<SoundEvent>();

            using (var stream = File.OpenRead(path))
            {
                using (var reader = new BinaryReader(stream, Encoding.Default, true))
                {
                    BufferMarkStart(stream);

                    int version = reader.ReadInt32();
                    if (version != 24)
                    {
                        throw new UnsupportedMetadataVersionException();
                    }

                    headerBuffer = BufferRead(stream);

                    // skip pck section
                    Output.Verbose(string.Format("Skipping PCK section at `{0}`", stream.Position));

                    BufferMarkStart(stream);

                    int pckCount = reader.ReadInt32();
                    for (var i = 0; i < pckCount; i++)
                    {
                        uint pckNameLength = reader.ReadUInt32();
                        stream.Seek(pckNameLength, SeekOrigin.Current);
                        stream.Seek(4, SeekOrigin.Current); // soundbank id ?
                    }

                    pckBuffer = BufferRead(stream);

                    // sound containers
                    Output.Verbose(string.Format("Processing sound containers at `{0}`", stream.Position));

                    int sndCount = reader.ReadInt32();
                    for (var i = 0; i < sndCount; i++)
                    {
                        uint sndNameLength = reader.ReadUInt32();
                        byte[] nameBytes = new byte[sndNameLength];
                        reader.Read(nameBytes, 0, (int)sndNameLength);
                        string containerName = Encoding.UTF8.GetString(nameBytes);

                        SoundContainer container = new SoundContainer
                        {
                            Name = containerName,
                            Parts = new List<SoundContainerPart>(),
                        };

                        Output.Debug(string.Format("Processing `[SoundContainer Name='{0}']`", containerName));

                        uint sndContainerCount = reader.ReadUInt32();
                        for (var j = 0; j < sndContainerCount; j++)
                        {
                            uint partId = reader.ReadUInt32();
                            uint maskLength = reader.ReadUInt32();

                            SoundContainerPart part = new SoundContainerPart
                            {
                                Id = partId,
                                ValidityMask = new List<uint>(),
                            };

                            Output.Debug(string.Format("Processing `[SoundContainerPart Id='{0}']`", partId));

                            for (var k = 0; k < maskLength; k++)
                            {
                                uint mask = reader.ReadUInt32();
                                part.ValidityMask.Add(mask);
                            }

                            container.Parts.Add(part);
                        }

                        soundContainers.Add(container);
                    }

                    Output.Verbose(string.Format("Extracted `{0}` sound containers", soundContainers.Count));

                    // skip bnk section
                    Output.Verbose(string.Format("Skipping BNK section at `{0}`", stream.Position));

                    BufferMarkStart(stream);

                    int bnkCount = reader.ReadInt32();
                    for (var i = 0; i < bnkCount; i++)
                    {
                        uint bnkNameLength = reader.ReadUInt32();
                        stream.Seek(bnkNameLength, SeekOrigin.Current);
                        stream.Seek(4, SeekOrigin.Current); // id
                    }

                    bnkBuffer = BufferRead(stream);

                    // skip aux bus section
                    Output.Verbose(string.Format("Skipping Aux Bus section at `{0}`", stream.Position));

                    BufferMarkStart(stream);

                    uint busCount = reader.ReadUInt32();
                    for (var i = 0; i < busCount; i++)
                    {
                        stream.Seek(4, SeekOrigin.Current); // id
                        uint busNameLength = reader.ReadUInt32();
                        stream.Seek(busNameLength, SeekOrigin.Current);
                    }

                    busBuffer = BufferRead(stream);

                    // skip rtpc section
                    Output.Verbose(string.Format("Skipping RTPC section at `{0}`", stream.Position));

                    BufferMarkStart(stream);

                    uint rtpcCount = reader.ReadUInt32();
                    for (var i = 0; i < rtpcCount; i++)
                    {
                        stream.Seek(4, SeekOrigin.Current); // id
                        uint rtpcNameLength = reader.ReadUInt32();
                        stream.Seek(rtpcNameLength, SeekOrigin.Current);
                    }

                    rtpcBuffer = BufferRead(stream);

                    // skip switch section
                    Output.Verbose(string.Format("Skipping switch group section at `{0}`", stream.Position));

                    BufferMarkStart(stream);

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

                    switchGroupBuffer = BufferRead(stream);

                    // skip state section
                    Output.Verbose(string.Format("Skipping state section at `{0}`", stream.Position));

                    BufferMarkStart(stream);

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

                    stateGroupBuffer = BufferRead(stream);

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

                    // skip event prefetch section
                    Output.Verbose(string.Format("Skipping event prefetch section at `{0}`", stream.Position));

                    // TODO How is prefetching used in the game anyways? Do we even want/need to support this for custom sounds?

                    BufferMarkStart(stream);

                    uint eventPrefetchCount = reader.ReadUInt32();
                    for (var i = 0; i < eventPrefetchCount; i++)
                    {
                        stream.Seek(4, SeekOrigin.Current); // id
                    }

                    eventPrefetchBuffer = BufferRead(stream);
                }
            }

            return new SoundMetadata
            {
                SoundContainers = soundContainers,
                PathParts = pathParts,
                SoundEvents = soundEvents,
                HeaderBuffer = headerBuffer,
                PckBuffer = pckBuffer,
                BnkBuffer = bnkBuffer,
                BusBuffer = busBuffer,
                RtpcBuffer = rtpcBuffer,
                SwitchGroupBuffer = switchGroupBuffer,
                StateGroupBuffer = stateGroupBuffer,
                EventPrefetchBuffer = eventPrefetchBuffer,
            };
        }
    }
}