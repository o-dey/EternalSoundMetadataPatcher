using EternalSoundMetadataPatcher.ConsoleIO;
using EternalSoundMetadataPatcher.Wwise.Generated;
using EternalSoundMetadataPatcher.Wwise.Structure.Audio;
using EternalSoundMetadataPatcher.Wwise.Structure.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EternalSoundMetadataPatcher.Wwise
{
    public class MediaProperties
    {
        public string Path;
        public float TrimBegin = 0;
        public float TrimEnd = 0;
        public float Duration = 0;
    }

    public class WwiseProject
    {
        public List<AudioFile> AudioFiles = new List<AudioFile>();
        public List<AudioObject> AudioObjects = new List<AudioObject>();
        public List<EventObject> EventObjects = new List<EventObject>();
        public SoundbanksInfo SoundbanksInfo;

        static public WwiseProject FromDirectory(string directory)
        {
            Output.Information($"Processing Wwise project at `{directory}`");

            // basic validation to drop out early with slightly helpful messaging

            string soundbanksInfoFilePath = Path.Combine(directory, @"GeneratedSoundBanks\Windows\SoundbanksInfo.xml");
            string attenuationsFilePath = Path.Combine(directory, @"Attenuations\Default Work Unit.wwu");
            string audioObjectsFilePath = Path.Combine(directory, @"Actor-Mixer Hierarchy\ghost_mod.wwu");
            string eventsFilePath = Path.Combine(directory, @"Events\mod_events.wwu");
            string externalSourcesFilePath = Path.Combine(directory, "..", @"ModWavs\test.wsources");

            if (!File.Exists(soundbanksInfoFilePath))
            {
                throw new FileNotFoundException(
                    $"Missing soundbanks info file `{soundbanksInfoFilePath}`. Make sure that you have packaged your Wwise project!"
                );
            }
            if (!File.Exists(attenuationsFilePath))
            {
                throw new FileNotFoundException(
                    $"Missing attenuations file `{attenuationsFilePath}`."
                );
            }
            if (!File.Exists(audioObjectsFilePath))
            {
                throw new FileNotFoundException(
                    $"Missing audio objects file `{audioObjectsFilePath}`. Make sure that you are using the correct mod sounds project!"
                );
            }
            if (!File.Exists(eventsFilePath))
            {
                throw new FileNotFoundException(
                    $"Missing events file `{eventsFilePath}`. Make sure that you are using the correct mod sounds project!"
                );
            }
            if (!File.Exists(externalSourcesFilePath))
            {
                throw new FileNotFoundException(
                    $"Missing external sources file `{externalSourcesFilePath}`. Make sure that you are using the correct mod sounds project!"
                );
            }

            // soundbank info for mods soundbank only

            SoundbanksInfo soundbanksInfo = SoundbanksInfo.FromFile(soundbanksInfoFilePath, x => x == "mods");

            // find all the audio files

            var audioFiles = new List<AudioFile>();

            XmlDocument externalSourcesDoc = new XmlDocument();
            externalSourcesDoc.Load(externalSourcesFilePath);

            Output.Verbose($"Extracting external sources from `{externalSourcesFilePath}`");

            XmlNodeList sourceNodes = externalSourcesDoc.DocumentElement.SelectNodes("Source");
            foreach (XmlNode sourceNode in sourceNodes)
            {
                string sourcePath = sourceNode.Attributes["Path"].Value;
                string sourceName = Path.GetFileNameWithoutExtension(sourcePath);

                Output.Debug($"Processing `<{sourceNode.LocalName} Path='{sourcePath}'>`");

                audioFiles.Add(new AudioFile { Path = sourcePath, Name = sourceName });
            }

            Output.Verbose($"Extracted `{audioFiles.Count}` external sources");

            string mediaFilesPath = Path.Combine(directory, @"originals\SFX");

            Output.Verbose($"Gathering media files from `{mediaFilesPath}`");

            string[] mediaFiles = Directory.GetFiles(mediaFilesPath, "*.wav");
            foreach (string mediaFile in mediaFiles)
            {
                string sourcePath = mediaFile;
                string sourceName = Path.GetFileNameWithoutExtension(mediaFile);

                Output.Debug($"Processing `[Media Path='{sourcePath}']`");

                audioFiles.Add(new AudioFile { Path = sourcePath, Name = sourceName });
            }

            Output.Verbose($"Found `{mediaFiles.Length}` media files");

            // gather audio objects

            Output.Verbose($"Extracting audio objects from `{audioObjectsFilePath}`");

            var attenuationsDoc = new XmlDocument();
            // TODO select by work unit id?
            attenuationsDoc.Load(attenuationsFilePath);

            var audioObjectsDoc = new XmlDocument();
            audioObjectsDoc.Load(audioObjectsFilePath);

            List<AudioObject> audioObjects = new List<AudioObject>();

            XmlNodeList mixerNodes = audioObjectsDoc.DocumentElement.SelectNodes("AudioObjects/WorkUnit/ChildrenList/ActorMixer");
            foreach (XmlNode mixerNode in mixerNodes)
            {
                audioObjects.AddRange(ProcessAudioObjectNode(mixerNode, attenuationsDoc, soundbanksInfo, audioFiles));
            }

            Output.Verbose($"Extracted `{audioObjects.Count}` audio objects");

            // gather event objects

            Output.Verbose($"Extracting event objects from `{eventsFilePath}`");

            var modEventsDoc = new XmlDocument();
            modEventsDoc.Load(eventsFilePath);

            var eventObjects = new List<EventObject>();

            string baseUnitName = modEventsDoc.DocumentElement.SelectSingleNode("Events/WorkUnit").Attributes["Name"].Value;

            XmlNodeList unitNodes = modEventsDoc.DocumentElement.SelectNodes("Events/WorkUnit/ChildrenList/WorkUnit");
            foreach (XmlNode unitNode in unitNodes)
            {
                string unitName = unitNode.Attributes["Name"].Value;

                var objectPathParts = new List<string> {
                    "Default Work Unit",
                    baseUnitName,
                    unitName
                };

                Output.Verbose($"Processing `<{unitNode.LocalName} Name='{unitName}' ID='{unitNode.Attributes["ID"].Value}'>`");

                // TODO sanitize unit name
                modEventsDoc.Load(Path.Combine(directory, $@"Events\{unitName}.wwu"));

                XmlNodeList eventNodes = modEventsDoc.DocumentElement.SelectNodes("Events/WorkUnit/ChildrenList/Event");
                foreach (XmlNode soundEventNode in eventNodes)
                {
                    string eventName = soundEventNode.Attributes["Name"].Value;
                    string eventGuid = soundEventNode.Attributes["ID"].Value;

                    Output.Debug($"Processing `<{soundEventNode.LocalName} Name='{eventName}' ID='{eventGuid}'>`");

                    if (!soundbanksInfo.EventIds.ContainsKey(eventGuid))
                    {
                        throw new Exception("no matching event guid");
                    }
                    uint eventId = soundbanksInfo.EventIds[eventGuid];

                    bool eventIsStopEvent = soundEventNode.SelectSingleNode("ChildrenList/Action/PropertyList/Property[@Name='ActionType']").Attributes["Value"].Value == "2";
                    EventType eventType = eventIsStopEvent ? EventType.Stop : EventType.Play;

                    XmlNode objectRefNode = soundEventNode.SelectSingleNode("ChildrenList/Action/ReferenceList/Reference/ObjectRef");
                    string eventTarget = objectRefNode.Attributes["Name"].Value;
                    string eventTargetGuid = objectRefNode.Attributes["ID"].Value;

                    XmlNode eventTargetAudioObjectNode = audioObjectsDoc.DocumentElement.SelectSingleNode($"//*[@ID='{eventTargetGuid}']");
                    if (eventTargetAudioObjectNode == null)
                    {
                        throw new Exception("no matching target audio node");
                    }

                    if (eventTargetAudioObjectNode.LocalName == "Sound")
                    {
                        // single sound

                        AudioObject eventTargetAudioObject = audioObjects.Find(x => x.Guid == eventTargetGuid);
                        if (eventTargetAudioObject == null)
                        {
                            throw new Exception("no matching audio object");
                        }

                        eventObjects.Add(
                            new EventObject
                            {
                                Type = eventType,
                                Id = soundbanksInfo.EventIds[eventGuid],
                                Guid = eventGuid,
                                TargetGuid = eventTargetGuid,
                                Name = eventName,
                                TargetAudioObject = eventTargetAudioObject,
                                AudioObjects = new List<AudioObject> { eventTargetAudioObject },
                                SoundbankId = soundbanksInfo.SoundbankIds["mods"],
                                PathParts = objectPathParts,
                            }
                        );
                    }
                    else
                    {
                        // possible multi sound

                        XmlNode childSoundContainerNode = audioObjectsDoc.DocumentElement.SelectSingleNode($"//*[@ID='{eventTargetGuid}']");

                        AudioObject eventTargetAudioObject = audioObjects.Find(x => x.Guid == eventTargetGuid);
                        if (eventTargetAudioObject == null)
                        {
                            throw new Exception("no matching audio object");
                        }

                        XmlNodeList childSoundNodes = audioObjectsDoc.DocumentElement.SelectNodes($"//*[@ID='{eventTargetGuid}']//Sound");
                        List<string> childSoundNodeGuids = new List<string>();
                        foreach (XmlNode childSoundNode in childSoundNodes)
                        {
                            childSoundNodeGuids.Add(childSoundNode.Attributes["ID"].Value);
                        }

                        List<AudioObject> eventTargetAudioObjects = audioObjects.FindAll(x => childSoundNodeGuids.IndexOf(x.Guid) != -1);
                        if (eventTargetAudioObjects.Count != childSoundNodes.Count)
                        {
                            throw new Exception("missing matching audio objects");
                        }

                        eventObjects.Add(
                            new EventObject
                            {
                                Type = eventType,
                                Id = soundbanksInfo.EventIds[eventGuid],
                                Guid = eventGuid,
                                TargetGuid = eventTargetGuid,
                                Name = eventName,
                                TargetAudioObject = eventTargetAudioObject,
                                AudioObjects = eventTargetAudioObjects,
                                SoundbankId = soundbanksInfo.SoundbankIds["mods"],
                                PathParts = objectPathParts,
                            }
                        );
                    }
                }
            }

            Output.Verbose($"Extracted `{eventObjects.Count}` event objects");

            // profit

            var project = new WwiseProject
            {
                AudioFiles = audioFiles,
                AudioObjects = audioObjects,
                EventObjects = eventObjects,
                SoundbanksInfo = soundbanksInfo,
            };

            return project;
        }

        static private List<AudioObject> ProcessAudioObjectNode(
            XmlNode node,
            XmlDocument attenuationsDoc,
            SoundbanksInfo soundbanksInfo,
            List<AudioFile> audioFiles,
            AudioObject parentAudioObject = null
        )
        {
            var audioObjects = new List<AudioObject>();

            string name = node.Attributes["Name"].Value;
            string guid = node.Attributes["ID"].Value;

            Output.Debug($"Processing `<{node.LocalName} Name='{name}' ID='{guid}'>`");

            float attenuation = 0;
            bool is2D = true;
            bool isLooping = node.SelectSingleNode("PropertyList/Property[@Name='IsLoopingEnabled']")?.Attributes["Value"].Value == "True";

            if (parentAudioObject != null)
            {
                attenuation = parentAudioObject.Attenuation;
                is2D = parentAudioObject.Is2D;
            }

            XmlNode overridePositioningNode = node.SelectSingleNode("PropertyList/Property[@Name='OverridePositioning']");
            bool overridePositioning = overridePositioningNode != null && overridePositioningNode.Attributes["Value"].Value == "True";

            if (parentAudioObject == null || overridePositioning)
            {
                XmlNode enableAttenuationNode = node.SelectSingleNode("PropertyList/Property[@Name='EnableAttenuation']");
                XmlNode attenuationRefNode = node.SelectSingleNode("ReferenceList/Reference[@Name='Attenuation']/ObjectRef");
                bool isAttenuationEnabled = attenuationRefNode != null && (enableAttenuationNode == null || enableAttenuationNode.Attributes["Value"].Value == "True");
                
                if (isAttenuationEnabled)
                {
                    string attenuationGuid = attenuationRefNode.Attributes["ID"].Value;

                    attenuation = 100; // default max distance for custom attenuation
                    string attenuationRadiusMax =
                        attenuationsDoc
                        .DocumentElement
                        .SelectSingleNode(
                            $"Attenuations/WorkUnit/ChildrenList/Attenuation[@ID='{attenuationGuid}']/PropertyList/Property[@Name='RadiusMax']"
                        )
                        ?.Attributes["Value"].Value;

                    if (attenuationRadiusMax != null)
                    {
                        attenuation = float.Parse(attenuationRadiusMax);
                    }
                }

                string spatilization = node.SelectSingleNode("PropertyList/Property[@Name='3DSpatialization']")?.Attributes["Value"].Value;
                is2D = spatilization == null || spatilization == "0";
            }

            var audioObject = new AudioObject
            {
                Guid = guid,
                Name = name,
                Attenuation = attenuation,
                Is2D = is2D,
                IsLooping = isLooping,
            };
            audioObjects.Add(audioObject);

            // There can be multiple sources per sound node, AFAIU this can be used to for example
            // define different sounds per platform and language, but we're not going to support it
            // until a need for it comes up and the modded game specifically supports it. For now we
            // simply use the first source in the list.
            //
            // https://www.audiokinetic.com/?action=showtopic&language=en&source=Help&version=v2019.2.5_7349&numid=4

            if (node.LocalName == "Sound")
            {
                string externalSourceGuid = node.SelectSingleNode("ChildrenList/ExternalSource")?.Attributes["ID"].Value;
                string mediaFileId = node.SelectSingleNode("ChildrenList/AudioFileSource/MediaIDList/MediaID")?.Attributes["ID"].Value;

                if (externalSourceGuid == null && mediaFileId == null)
                {
                    // neither external nor embedded sound references are present, usually this means
                    // that no sources have been assigned to the event
                    throw new Exception($"Missing source for `{name}` audio object");
                }

                if (externalSourceGuid != null)
                {
                    AudioFile audioFile = audioFiles.Find(x => x.Name == name);
                    if (audioFile == null)
                    {
                        // no source with a matching sound name was found in the test.wsources file,
                        // this might be due to not following the naming conventions that require
                        // the file to be named the same as the audio object in wwise, or actually
                        // no source having been set up in the test.wsources files
                        throw new Exception($"Missing external audio source for `{name}` audio object");
                    }

                    audioObject.Source = new ExternalAudioSource
                    {
                        Id = soundbanksInfo.ExternalSources[externalSourceGuid].Id,
                        Guid = externalSourceGuid,
                        Path = audioFile.Path,
                    };
                }
                else if (mediaFileId != null)
                {
                    string trimBeginValue = node.SelectSingleNode("ChildrenList/AudioFileSource/PropertyList/Property[@Name='TrimBegin']")?.Attributes["Value"].Value;
                    string timEndValue = node.SelectSingleNode("ChildrenList/AudioFileSource/PropertyList/Property[@Name='TrimEnd']")?.Attributes["Value"].Value;

                    float.TryParse(trimBeginValue, NumberStyles.Number, CultureInfo.InvariantCulture, out float trimBegin);
                    float.TryParse(timEndValue, NumberStyles.Number, CultureInfo.InvariantCulture, out float trimEnd);

                    string language = node.SelectSingleNode("ChildrenList/AudioFileSource/Language").InnerText;
                    string audioFilePath = node.SelectSingleNode("ChildrenList/AudioFileSource/AudioFile").InnerText;
                    MediaFile mediaFile = soundbanksInfo.MediaFiles[mediaFileId];

                    audioObject.Source = new EmbeddedAudioSource
                    {
                        Id = mediaFile.Id,
                        Path = Path.Combine(@"originals", language, audioFilePath),
                        TrimBegin = trimBegin,
                        TrimEnd = trimEnd,
                        Duration = mediaFile.Duration,
                    };
                }
            }
            else
            {
                XmlNodeList childNodes = node.SelectNodes("ChildrenList/*");
                if (childNodes.Count != 0)
                {
                    foreach (XmlNode childNode in childNodes)
                    {
                        audioObjects.AddRange(ProcessAudioObjectNode(childNode, attenuationsDoc, soundbanksInfo, audioFiles, audioObject));
                    }
                }
            }

            return audioObjects;
        }
    }
}