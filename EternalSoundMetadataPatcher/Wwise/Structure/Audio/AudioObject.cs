using System;

namespace EternalSoundMetadataPatcher.Wwise.Structure.Audio
{
    public class AudioObject
    {
        public string Guid;
        public string Name;
        public float Attenuation;
        public bool Is2D;
        public bool IsLooping;
        public AudioSource Source;

        public bool Equals(AudioObject other)
        {
            var equal = 
                other != null &&
                other.Guid == Guid &&
                other.Name == Name &&
                other.Attenuation == Attenuation &&
                other.Is2D == Is2D &&
                other.IsLooping == IsLooping;

            if (Source != null)
            {
                equal = equal && other.Source.Equals(Source);
            }

            return equal;
        }

        public override bool Equals(Object obj)
        {
            var other = obj as AudioObject;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public override int GetHashCode()
        {
            var hash =
                Guid.GetHashCode() ^
                Name.GetHashCode() ^
                Attenuation.GetHashCode() ^
                Is2D.GetHashCode() ^
                IsLooping.GetHashCode();

            if (Source != null)
            {
                hash ^= Source.GetHashCode();
            }

            return hash;
        }
    }
}
