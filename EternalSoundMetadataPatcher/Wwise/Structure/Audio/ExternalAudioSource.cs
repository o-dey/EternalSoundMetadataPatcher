using System;

namespace EternalSoundMetadataPatcher.Wwise.Structure.Audio
{
    public class ExternalAudioSource : AudioSource
    {
        public string Guid;

        public bool Equals(ExternalAudioSource other)
        {
            return
                other != null &&
                other.Id == Id &&
                other.Path == Path &&
                other.Guid == Guid;
        }

        public override bool Equals(Object obj)
        {
            var other = obj as ExternalAudioSource;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return
                Id.GetHashCode() ^
                Path.GetHashCode() ^
                Guid.GetHashCode();
        }
    }
}
