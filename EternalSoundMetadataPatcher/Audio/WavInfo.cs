using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EternalSoundMetadataPatcher.Audio
{
    public class WavInfo
    {
        /// <summary>
        /// The duration in seconds
        /// </summary>
        public readonly float Duration;

        private WavInfo(float duration)
        {
            Duration = duration;
        }

        /// <summary>
        /// Reads meta information from a WAV file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>A WavInfo instance holding the extracted meta information.</returns>
        /// <exception cref="MissingRiffMarkerException">When the RIFF marker couldn't be found.</exception>
        /// <exception cref="MissingWaveMarkerException">When the WAVE marker couldn't be found.</exception>
        /// <exception cref="MissingFmtChunkException">When the FMT chunk couldn't be found.</exception>
        /// <exception cref="MissingDataChunkException">When the data chunk couldn't be found.</exception>
        public static WavInfo FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream, Encoding.Default, false))
                {
                    string identifier = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    if (identifier != "RIFF")
                    {
                        throw new MissingRiffMarkerException($"Missing RIFF marker in `{path}`");
                    }

                    uint size = reader.ReadUInt32();

                    string format = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    if (format != "WAVE")
                    {
                        throw new MissingWaveMarkerException($"Missing WAVE marker in `{path}`");
                    }

                    bool foundFormatChunk = false;
                    bool foundDataChunk = false;

                    uint byteRate = 0;
                    uint dataChunkSize = 0;

                    while (stream.Position < stream.Length)
                    {
                        string chunkID = Encoding.ASCII.GetString(reader.ReadBytes(4));
                        uint chunkSize = reader.ReadUInt32();

                        if (chunkID == "fmt ")
                        {
                            foundFormatChunk = true;

                            reader.ReadUInt16(); // audio format
                            reader.ReadUInt16(); // number of channels
                            reader.ReadUInt32(); // sample rate
                            byteRate = reader.ReadUInt32();
                            reader.ReadUInt16(); // block align
                            reader.ReadUInt16(); // bits per sample

                            uint padding = chunkSize - 16;
                            if (padding != 0)
                            {
                                stream.Seek(padding, SeekOrigin.Current);
                            }

                            continue;
                        }

                        if (chunkID == "data")
                        {
                            foundDataChunk = true;

                            dataChunkSize = chunkSize;
                            stream.Seek(dataChunkSize, SeekOrigin.Current);

                            continue;
                        }

                        if (foundFormatChunk && foundDataChunk)
                        {
                            break;
                        }

                        // skip chunk
                        stream.Seek(chunkSize, SeekOrigin.Current);
                    }

                    if (!foundFormatChunk)
                    {
                        throw new MissingFmtChunkException($"Missing FMT chunk in `{path}`");
                    }

                    if (!foundDataChunk)
                    {
                        throw new MissingDataChunkException($"Missing data chunk in `{path}`");
                    }

                    float duration = (float)dataChunkSize / (float)byteRate;

                    return new WavInfo(duration);
                }
            }
        }
    }
}
