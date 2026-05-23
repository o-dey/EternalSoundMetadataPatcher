using System;

namespace EternalSoundMetadataPatcher.Wwise
{
    public class AudioFile
    {
        public string Path;
        public string Name;

        public bool Equals(AudioFile other)
        {
            return
                other != null &&
                other.Path == Path &&
                other.Name == Name;
        }

        public override bool Equals(Object obj)
        {
            var other = obj as AudioFile;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return
                Path.GetHashCode() ^
                Name.GetHashCode();
        }
    }
}
