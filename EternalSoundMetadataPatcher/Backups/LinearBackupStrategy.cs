using EternalSoundMetadataPatcher.ConsoleIO;
using System;
using System.IO;
using System.Linq;

namespace EternalSoundMetadataPatcher.Backups
{
    public class LinearBackupStrategy : IBackupStrategy
    {
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
            string backupFilePath = Path.Combine(directory, $"{filename}.bak{backupNum}");

            Output.Information(string.Format("Creating backup `{0}`", backupFilePath));

            File.Copy(path, backupFilePath, true);
            File.SetLastWriteTimeUtc(backupFilePath, DateTime.UtcNow);
        }
    }
}
