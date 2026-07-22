using EternalSoundMetadataPatcher.Wwise.Structure.Audio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EternalSoundMetadataPatcher.Wwise.Structure.Events
{
    public class EventObject
    {
        public EventType Type;
        public uint Id;
        public string Guid;
        public string TargetGuid;
        public string Name;
        public List<string> PathParts;
        public AudioObject TargetAudioObject;
        public List<AudioObject> AudioObjects;
        public uint SoundbankId;
    }
}
