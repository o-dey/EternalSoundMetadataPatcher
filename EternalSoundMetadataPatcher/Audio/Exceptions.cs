using System;

namespace EternalSoundMetadataPatcher.Audio
{
    public class MissingRiffMarkerException : FormatException
    {
        public MissingRiffMarkerException(string message) : base(message) { }
    }

    public class MissingWaveMarkerException : FormatException
    {
        public MissingWaveMarkerException(string message) : base(message) { }
    }

    public class MissingFmtChunkException : FormatException
    {
        public MissingFmtChunkException(string message) : base(message) { }
    }

    public class MissingDataChunkException : FormatException
    {
        public MissingDataChunkException(string message) : base(message) { }
    }
}
