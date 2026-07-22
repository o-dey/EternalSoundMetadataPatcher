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
    }
}