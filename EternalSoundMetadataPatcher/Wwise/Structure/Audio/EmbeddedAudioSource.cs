using System;

namespace EternalSoundMetadataPatcher.Wwise.Structure.Audio
{
    public class EmbeddedAudioSource : AudioSource
    {
        public float TrimBegin = 0;
        public float TrimEnd = 0;
        public float Duration = 0;

        public bool Equals(EmbeddedAudioSource other)
        {
            return
                other != null &&
                other.Id == Id &&
                other.Path == Path &&
                other.TrimBegin == TrimBegin &&
                other.TrimEnd == TrimEnd &&
                other.Duration == Duration;
        }

        public override bool Equals(Object obj)
        {
            var other = obj as EmbeddedAudioSource;
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
                TrimBegin.GetHashCode() ^
                TrimEnd.GetHashCode() ^
                Duration.GetHashCode();
        }
    }
}
