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
    }
}