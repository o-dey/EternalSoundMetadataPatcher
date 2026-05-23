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

        public bool Equals(EventObject other)
        {
            var equal =
                other != null &&
                other.Type == Type&&
                other.Id == Id &&
                other.Guid == Guid &&
                other.TargetGuid == TargetGuid &&
                other.Name == Name &&
                other.PathParts.SequenceEqual(PathParts) &&
                other.TargetAudioObject.Equals(TargetAudioObject) &&
                other.AudioObjects.SequenceEqual(AudioObjects) &&
                other.SoundbankId == SoundbankId;

            return equal;
        }

        public override bool Equals(Object obj)
        {
            var other = obj as EventObject;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public override int GetHashCode()
        {
            var hash =
                Type.GetHashCode() ^
                Id.GetHashCode() ^
                Guid.GetHashCode() ^
                TargetGuid.GetHashCode() ^
                Name.GetHashCode();

            foreach (var x in PathParts)
            {
                hash ^= x.GetHashCode();
            }

            hash ^= TargetAudioObject.GetHashCode();

            foreach (var x in AudioObjects)
            {
                hash ^= x.GetHashCode();
            }

            hash ^= SoundbankId.GetHashCode();

            return hash;
        }
    }
}
