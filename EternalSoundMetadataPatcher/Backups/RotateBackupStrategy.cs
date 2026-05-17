using EternalSoundMetadataPatcher.ConsoleIO;
using System;
using System.IO;
using System.Linq;

namespace EternalSoundMetadataPatcher.Backups
{
    public class RotateBackupStrategy : IBackupStrategy
    {
        /// <summary>
        /// The maximum number of backups. When the limit is exceeded,
        /// backup filenames will rotate and start back at 1.
        /// </summary>
        public int Limit = 10;

        public RotateBackupStrategy() { }

        public RotateBackupStrategy(int limit)
        {
            Limit = limit;
        }

        public void CreateNew(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileName(path);

            string[] backupFiles =
                Directory
                .GetFiles(directory, $"{filename}.bak*")
                .OrderBy(x => File.GetLastWriteTimeUtc(x))
                .ToArray();

            int backupNum = backupFiles.Length + 1;
            if (backupFiles.Length > 0)
            {
                string lastBackupFile = backupFiles[backupFiles.Length - 1];
                int lastBackupNum = int.Parse(lastBackupFile.Substring(lastBackupFile.Length - 1));
                backupNum = lastBackupNum + 1;
            }
            if (backupNum > Limit)
            {
                backupNum = 1;
            }

            string backupFilePath = Path.Combine(directory, $"{filename}.bak{backupNum}");

            Output.Information(string.Format("Creating backup `{0}`", backupFilePath));

            File.Copy(path, backupFilePath, true);
            File.SetLastWriteTimeUtc(backupFilePath, DateTime.UtcNow);
        }
    }
}
