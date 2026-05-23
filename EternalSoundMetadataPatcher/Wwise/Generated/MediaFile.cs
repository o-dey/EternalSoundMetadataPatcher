using System;

namespace EternalSoundMetadataPatcher.Wwise.Generated
{
    /// <summary>
    /// Data of a `<File>` node in `<IncludedMemoryFiles>` or `<ReferencedStreamedFiles>`.
    /// </summary>
    public class MediaFile
    {
        /// <summary>
        /// The media asset id that references the embedded media file.
        /// </summary>
        public uint Id;

        /// <summary>
        /// The relative path to the media file, seemingly reflecting the structure in the Wwise project's `originals` folder.
        /// </summary>
        public string Path;

        /// <summary>
        /// The base filename.
        /// </summary>
        public string Name;

        /// <summary>
        /// The duration in seconds, as defined on the parent `<Event>` node.
        /// </summary>
        public float Duration;

        public bool Equals(MediaFile other)
        {
            return
                other != null &&
                other.Id == Id &&
                other.Path == Path &&
                other.Name == Name &&
                other.Duration == Duration;
        }

        public override bool Equals(Object obj)
        {
            var other = obj as MediaFile;
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
                Name.GetHashCode() ^
                Duration.GetHashCode();
        }
    }
}