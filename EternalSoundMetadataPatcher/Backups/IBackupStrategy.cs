namespace EternalSoundMetadataPatcher.Backups
{
    public interface IBackupStrategy
    {
        /// <summary>
        /// Creates a new backup of the file given in <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The file to backup.</param>
        void CreateNew(string path);
    }
}
