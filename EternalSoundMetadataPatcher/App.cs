using EternalSoundMetadataPatcher.Backups;
using EternalSoundMetadataPatcher.ConsoleIO;
using EternalSoundMetadataPatcher.Patching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EternalSoundMetadataPatcher
{
    public class App
    {
        public static int Main(string[] args)
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();

            bool showHelp = false;
            int backupsValue = 10;
            bool applySoundContainerFix = true;

            Output.Level = OutputLevel.Normal;

            List<string> positionalArgs = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-h":
                        showHelp = true;
                        break;

                    case "-b":
                        if (args.Length <= i + 1)
                        {
                            Output.Error("-b argument value missing");
                            return 1;
                        }
                        if (!int.TryParse(args[++i], out backupsValue))
                        {
                            Output.Error("-b argument value invalid");
                            return 1;
                        }

                        if (backupsValue < -1)
                        {
                            Output.Error("-b argument value out of range");
                            return 1;
                        }
                        break;

                    case "-v":
                        Output.Level = OutputLevel.Verbose;
                        break;

                    case "-d":
                        Output.Level = OutputLevel.Debug;
                        break;

                    case "-no-snd-fix":
                        applySoundContainerFix = false;
                        break;

                    default:
                        positionalArgs.Add(args[i]);
                        break;
                }
            }

            if (positionalArgs.Count < 2 || showHelp)
            {
                Output.Information(
                    $"Usage: {assemblyName.Name} [-h] [-b] [-v] [-d] [-no-snd-fix] " +
                    "<path to idstudio mod directory> <path to Wwise project directory>",
                    2
                );
                Output.Information("Options:");
                Output.Information("\t-h\tDisplay this help.");
                Output.Information(
                    "\t-b\tBackup mode/limit (defaults to 10). -1 for \"unlimited\" backup files. 0 to disable backups.\r\n" +
                    "\t\t>= 1 to define the maximum number of backups (when the limit is reached, backup names will rotate)."
                );
                Output.Information("\t-v\tShow verbose output.");
                Output.Information("\t-d\tShow debug level output.");
                Output.Information("\t-no-snd-fix\tDisable sound container fix.");

                return 0;
            }

            string idStudioModDirectory = Path.GetFullPath(positionalArgs[0]);
            string wwiseDirectory = Path.GetFullPath(positionalArgs[1]);

            IBackupStrategy strategy = null;
            switch (backupsValue)
            {
                case 0: break;

                case -1:
                    strategy = new LinearBackupStrategy();
                    break;

                default:
                    strategy = new RotateBackupStrategy(backupsValue);
                    break;
            }

            Output.Information($"Running {assemblyName.Name} v{assemblyName.Version} on");
            Output.Information($"\tidStudio mod directory: {idStudioModDirectory}");
            Output.Information($"\tWwise project directory: {wwiseDirectory}");
            Output.Information($"\tApply sound container fix: {(applySoundContainerFix ? "yes" : "no")}");

            Patcher.Patch(idStudioModDirectory, wwiseDirectory, strategy, applySoundContainerFix);

            return 0;
        }
    }
}
