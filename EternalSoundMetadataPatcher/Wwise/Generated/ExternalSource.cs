using System;

namespace EternalSoundMetadataPatcher.Wwise.Generated
{
    /// <summary>
    /// Data of a `<source>` node in `<ExternalSources>`.
    /// </summary>
    public class ExternalSource
    {
        /// <summary>
        /// The media asset id for the releated `.wem` media file.
        /// </summary>
        public uint Id;

        /// <summary>
        /// The authoring ID.
        /// </summary>
        public string Guid;

        /// <summary>
        /// The name of the source.
        /// </summary>
        public string Name;

        public bool Equals(ExternalSource other)
        {
            return
                other != null &&
                other.Id == Id &&
                other.Guid == Guid &&
                other.Name == Name;
        }

        public override bool Equals(Object obj)
        {
            var other = obj as ExternalSource;
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
                Guid.GetHashCode() ^
                Name.GetHashCode();
        }
    }
}