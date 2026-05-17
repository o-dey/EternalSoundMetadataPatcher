using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EternalSoundMetadataPatcher.Metadata
{
    public class SoundEvent
    {
        public uint Id;
        public string Name;
        public float Attenuation;
        public bool Is2D;
        public bool IsLooping;
        public bool IsExternalSource;
        public uint Duration;
        public uint StopEventId;
        public List<uint> SoundIds;
        public List<uint> SoundbankIds;
        public List<string> PathParts;
    }
}
