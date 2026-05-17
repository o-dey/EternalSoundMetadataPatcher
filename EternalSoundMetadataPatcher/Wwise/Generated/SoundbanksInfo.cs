using EternalSoundMetadataPatcher.ConsoleIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace EternalSoundMetadataPatcher.Wwise.Generated
{
    public class SoundbanksInfo
    {
        public Dictionary<string, uint> SoundbankIds;
        public Dictionary<string, uint> EventIds;
        public Dictionary<string, ExternalSource> ExternalSources;
        public Dictionary<string, MediaFile> MediaFiles;
        public List<string> ObjectPathParts;

        private SoundbanksInfo() { }

        static public SoundbanksInfo FromFile(string path)
        {
            return FromFile(path, x => true);
        }

        static public SoundbanksInfo FromFile(string path, Func<string, bool> bankFilter)
        {
            Output.Information(string.Format("Processing soundbanks info file `{0}`", path));

            var doc = new XmlDocument();
            doc.Load(path);

            var soundbankIds = new Dictionary<string, uint>();
            var eventIds = new Dictionary<string, uint>();
            var externalSources = new Dictionary<string, ExternalSource>();
            var mediaFiles = new Dictionary<string, MediaFile>();
            var pathParts = new List<string>();

            XmlNodeList bankNodes = doc.DocumentElement.SelectNodes("SoundBanks/SoundBank");
            foreach (XmlNode bankNode in bankNodes)
            {
                uint soundbankId = uint.Parse(bankNode.Attributes["Id"].Value);
                string shoundbankName = bankNode.SelectSingleNode("ShortName").InnerText;

                if (!bankFilter(shoundbankName))
                {
                    Output.Verbose($"Discarding `<{bankNode.LocalName} Name='{shoundbankName}' Id='{soundbankId}'>`");
                    continue;
                }

                Output.Verbose($"Processing `<{bankNode.LocalName} Name='{shoundbankName}' Id='{soundbankId}'>`");

                soundbankIds[shoundbankName] = soundbankId;

                foreach (XmlNode eventNode in bankNode.SelectNodes("IncludedEvents/Event"))
                {
                    uint bankEventId = uint.Parse(eventNode.Attributes["Id"].Value);
                    string bankEventName = eventNode.Attributes["Name"].Value;
                    string bankEventGuid = eventNode.Attributes["GUID"].Value;
                    eventIds[bankEventGuid] = bankEventId;

                    Output.Debug($"Processing `<{eventNode.LocalName} Name='{bankEventName}' Id='{bankEventId}'>`");

                    // TODO does the path for events always have to start with `\Events\`?
                    string objectPath = eventNode.Attributes["ObjectPath"].Value;
                    if (objectPath.IndexOf(@"\Events\") == 0)
                    {
                        objectPath = objectPath.Substring(@"\Events\".Length);
                    }

                    string[] parts = objectPath.Split('\\');
                    parts = parts.Take(parts.Length - 1).ToArray(); // the final leaf is the event name

                    // TODO can this actually ever be 0? and if it can, should it be treated as an error?
                    if (parts.Length > 0)
                    {
                        pathParts.AddRange(parts);
                    }

                    // TODO test with all container types!
                    foreach (XmlNode mediaFileNode in eventNode.SelectNodes("(IncludedMemoryFiles|ReferencedStreamedFiles)/File"))
                    {
                        uint soundFileId = uint.Parse(mediaFileNode.Attributes["Id"].Value);
                        string mediaName = mediaFileNode.SelectSingleNode("ShortName").InnerText;

                        Output.Debug($"Processing `<{mediaFileNode.LocalName} Id='{soundFileId}' Name='{mediaName}'>`");

                        string mediaPath = mediaFileNode.SelectSingleNode("Path").InnerText;
                        float maxDuration = 0;

                        // The attribute may not exist, it can hold strings like `Infinite`, and it can be 0 for idk what reasons
                        XmlNode durationMaxMode = eventNode.Attributes["DurationMax"];
                        if (durationMaxMode != null)
                        {
                            float.TryParse(durationMaxMode.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out maxDuration);
                        }

                        mediaFiles[soundFileId.ToString()] = new MediaFile
                        {
                            Id = soundFileId,
                            Path = mediaPath,
                            Name = mediaName,
                            Duration = maxDuration,
                        };
                    }
                }

                foreach (XmlNode soundNode in bankNode.SelectNodes("ExternalSources/Source"))
                {
                    uint bankSoundId = uint.Parse(soundNode.Attributes["Id"].Value);
                    string bankSoundName = soundNode.Attributes["Name"].Value;
                    string bankSoundGuid = soundNode.Attributes["GUID"].Value;

                    Output.Debug($"Processing `<{soundNode.LocalName} Name='{bankSoundName}' Id='{bankSoundId}'>`");

                    externalSources[bankSoundGuid] = new ExternalSource
                    {
                        Id = bankSoundId,
                        Guid = bankSoundGuid,
                        Name = bankSoundName,
                    };
                }
            }

            pathParts = pathParts.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            Output.Verbose($"Extracted `{soundbankIds.Count}` soundbank IDs");
            Output.Verbose($"Extracted `{eventIds.Count}` event IDs");
            Output.Verbose($"Extracted `{externalSources.Count}` external sources");
            Output.Verbose($"Extracted `{mediaFiles.Count}` media files");
            Output.Verbose($"Extracted `{pathParts.Count}` object path parts");

            return new SoundbanksInfo
            {
                SoundbankIds = soundbankIds,
                EventIds = eventIds,
                ExternalSources = externalSources,
                MediaFiles = mediaFiles,
                ObjectPathParts = pathParts,
            };
        }
    }
}