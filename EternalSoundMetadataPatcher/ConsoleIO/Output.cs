using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EternalSoundMetadataPatcher.ConsoleIO
{
    public static class Output
    {
        public static OutputLevel Level = OutputLevel.Normal;

        public static void Information(string message, int newlines = 1)
        {
            Out(message, newlines, OutputLevel.Normal);
        }

        public static void Verbose(string message, int newlines = 1)
        {
            Out(message, newlines, OutputLevel.Verbose);
        }

        public static void Debug(string message, int newlines = 1)
        {
            Out(message, newlines, OutputLevel.Debug);
        }

        public static void Error(string message, int newlines = 1)
        {
            Err("ERROR: " + message, newlines);
        }

        private static void Out(string message, int newlines, OutputLevel level)
        {
            if (Level >= level)
            {
                if (newlines > 1)
                {
                    message += string.Concat(Enumerable.Repeat("\r\n", newlines - 1));
                }
                Console.Out.WriteLine(message);
                System.Diagnostics.Debug.WriteLine(message);
            }
        }

        private static void Err(string message, int newlines)
        {
            if (newlines > 1)
            {
                message += string.Concat(Enumerable.Repeat("\r\n", newlines - 1));
            }
            Console.Error.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
