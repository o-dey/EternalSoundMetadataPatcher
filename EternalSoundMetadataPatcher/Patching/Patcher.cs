using EternalSoundMetadataPatcher.Audio;
using EternalSoundMetadataPatcher.Backups;
using EternalSoundMetadataPatcher.ConsoleIO;
using EternalSoundMetadataPatcher.Metadata;
using EternalSoundMetadataPatcher.Metadata.Fixes;
using EternalSoundMetadataPatcher.Wwise;
using EternalSoundMetadataPatcher.Wwise.Generated;
using EternalSoundMetadataPatcher.Wwise.Structure.Audio;
using EternalSoundMetadataPatcher.Wwise.Structure.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EternalSoundMetadataPatcher.Patching
{
    public class Patcher
    {
        static public void Patch(string idStudioModDirectory, string wwiseDirectory, IBackupStrategy backupStrategy = null, bool soundContainerFix = false)
        {
            string soundbanksInfoFilePath = Path.Combine(idStudioModDirectory, @"base\sound\soundbanks\SoundbanksInfo.xml");
            string soundMetadataPath = Path.Combine(idStudioModDirectory, @"base\sound\soundbanks\pc\soundmetadata.bin");

            if (!File.Exists(soundbanksInfoFilePath))
            {
                throw new FileNotFoundException(
                    $"Missing mod soundbanks info file `{soundbanksInfoFilePath}`."
                );
            }
            if (!File.Exists(soundMetadataPath))
            {
                throw new FileNotFoundException(
                    $"Missing mod sound metadata file `{soundMetadataPath}`."
                );
            }

            SoundbanksInfo soundbanksInfo = SoundbanksInfo.FromFile(
                soundbanksInfoFilePath,
                x => x != "mods" && x != "music_mods"
            );
            SoundMetadata soundMetadata = SoundMetadata.FromFile(soundMetadataPath);
            WwiseProject project = WwiseProject.FromDirectory(wwiseDirectory);

            Output.Information("Patching sound metadata");

            var modSoundEvents = new List<SoundEvent>();

            foreach (EventObject eventObject in project.EventObjects)
            {
                //List<AudioObject> audioObjects = eventObject.AudioObjects;
                //float maxAttenuation = eventObject.AudioObjects.Max(x => x.Attenuation);

                float maxDuration = 0F;
                foreach (AudioObject audioObject in eventObject.AudioObjects)
                {
                    EmbeddedAudioSource embeddedAudioSource = audioObject.Source as EmbeddedAudioSource;

                    // if duration is already present, it stems from soundbank info for embedded media files,
                    // and it already has possible trimming applied
                    if (embeddedAudioSource != null && embeddedAudioSource.Duration != 0)
                    {
                        maxDuration = Math.Max(maxDuration, embeddedAudioSource.Duration);
                    }
                    else
                    {
                        // no duration being present is the norm for external sources, but apparently it can also
                        // be the case for embedded media files for idk what reason
                        string mediaFilePath = Path.Combine(wwiseDirectory, audioObject.Source.Path);

                        Output.Debug($"Reading WAV meta information from `{mediaFilePath}`");

                        var duration = WavInfo.FromFile(mediaFilePath).Duration;

                        // trim info is only present for embedded media files
                        if (embeddedAudioSource != null)
                        {
                            if (embeddedAudioSource.TrimEnd > 0)
                            {
                                duration -= duration - embeddedAudioSource.TrimEnd;
                            }
                            if (embeddedAudioSource.TrimBegin > 0)
                            {
                                duration -= embeddedAudioSource.TrimBegin;
                            }
                        }

                        maxDuration = Math.Max(maxDuration, duration);
                    }
                }

                bool targetIsExternalSource = eventObject.TargetAudioObject.Source is ExternalAudioSource;

                var soundEvent = new SoundEvent
                {
                    Id = eventObject.Id,
                    Name = eventObject.Name,
                    // TODO use max attenuation from individual audio objects instead?
                    Attenuation = eventObject.TargetAudioObject.Attenuation,
                    Is2D = eventObject.TargetAudioObject.Is2D,
                    IsLooping = eventObject.TargetAudioObject.IsLooping,
                    IsExternalSource = targetIsExternalSource,
                    Duration = (uint)(maxDuration * 1000),
                    StopEventId = 0,
                    SoundIds = eventObject.AudioObjects.Select(x => x.Source.Id).ToList(),
                    SoundbankIds = new List<uint> { eventObject.SoundbankId },
                    PathParts = eventObject.PathParts,
                };

                if (eventObject.Type == EventType.Play)
                {
                    EventObject stopEventObject =
                        project
                        .EventObjects
                        .Find(x => x.TargetGuid == eventObject.TargetGuid && x.Type == EventType.Stop);

                    if (stopEventObject != null)
                    {
                        soundEvent.StopEventId = stopEventObject.Id;
                    }
                }

                modSoundEvents.Add(soundEvent);
            }

            List<string> pathParts = new List<string>();
            pathParts.AddRange(soundbanksInfo.ObjectPathParts);
            pathParts.AddRange(project.SoundbanksInfo.ObjectPathParts);
            pathParts = pathParts.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            soundMetadata.PathParts = pathParts;

            List<SoundEvent> soundEvents =
                soundMetadata.SoundEvents
                .Where(x => x.PathParts.Count < 2 || x.PathParts[1] != "mod_events")
                .ToList();

            soundEvents.AddRange(modSoundEvents);

            // events must be ordered ascending by their id value, failing to do so will
            // results in sounds not playing
            soundMetadata.SoundEvents = soundEvents.OrderBy(x => x.Id).ToList();

            // Fix missing sound container vailidity masks (discovered by FlavorfulGecko5),
            // this should fix missing music and VO, specifically in TAG and Horde mode.
            //
            // Temporarily optionally for testing purposes, but in the end we probably want
            // to make it a permanent fix.
            if (soundContainerFix)
            {
                Output.Information("Applying sound container fix");

                SoundMetadataFixes.FixSoundContainerValidityMasks(soundMetadata);
            }

            if (backupStrategy != null)
            {
                backupStrategy.CreateNew(soundMetadataPath);
            }

            soundMetadata.WriteTo(soundMetadataPath);

            Output.Information("Patching complete");
        }
    }
}
