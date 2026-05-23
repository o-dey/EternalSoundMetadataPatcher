using EternalSoundMetadataPatcher.Wwise;
using EternalSoundMetadataPatcher.Wwise.Generated;
using EternalSoundMetadataPatcher.Wwise.Structure.Audio;
using EternalSoundMetadataPatcher.Wwise.Structure.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EternalSoundMetadataPatcher.Test
{
    [TestClass]
    public class WwiseProjectTests
    {
        [TestMethod]
        [DeploymentItem("TestData/Wwise/Projects/Default/Wwise Music Mod", "Default")]
        public void TestDefaultTemplate()
        {
            WwiseProject project = WwiseProject.FromDirectory(@"Default\ModsWwise");

            Assert.AreEqual(133, project.AudioFiles.Count);
            Assert.AreEqual(144, project.AudioObjects.Count);
            Assert.AreEqual(71, project.EventObjects.Count);

            Assert.AreEqual(1, project.SoundbanksInfo.SoundbankIds.Count);
            Assert.AreEqual(71, project.SoundbanksInfo.EventIds.Count);
            Assert.AreEqual(113, project.SoundbanksInfo.ExternalSources.Count);
            Assert.AreEqual(0, project.SoundbanksInfo.MediaFiles.Count);
            Assert.AreEqual(6, project.SoundbanksInfo.ObjectPathParts.Count);
        }

        [TestMethod]
        [DeploymentItem("TestData/Wwise/Projects/Custom/Wwise Music Mod", "Custom")]
        public void TestCustom()
        {
            WwiseProject project = WwiseProject.FromDirectory(@"Custom\ModsWwise");

            Assert.AreEqual(7, project.AudioFiles.Count);
            Assert.AreEqual(7, project.AudioObjects.Count);
            Assert.AreEqual(6, project.EventObjects.Count);

            Assert.AreEqual(1, project.SoundbanksInfo.SoundbankIds.Count);
            Assert.AreEqual(6, project.SoundbanksInfo.EventIds.Count);
            Assert.AreEqual(3, project.SoundbanksInfo.ExternalSources.Count);
            Assert.AreEqual(2, project.SoundbanksInfo.MediaFiles.Count);
            Assert.AreEqual(4, project.SoundbanksInfo.ObjectPathParts.Count);

            var expectedAudioFiles = new List<AudioFile> {
                new AudioFile {
                    Name="generic1",
                    Path="..\\ModWavs\\generic\\generic1.wav"
                },
                new AudioFile {
                    Name="generic2",
                    Path="..\\ModWavs\\generic\\generic2.wav"
                },
                new AudioFile {
                    Name="generic3",
                    Path="..\\ModWavs\\generic\\generic3.wav"
                },
                new AudioFile {
                    Name="ambient_01",
                    Path="Custom\\ModsWwise\\originals\\SFX\\ambient_01.wav"
                },
                new AudioFile {
                    Name="ambient_02",
                    Path="Custom\\ModsWwise\\originals\\SFX\\ambient_02.wav"
                },
                new AudioFile {
                    Name="vo_4TH_PRIEST__Here_to_lecture_us",
                    Path="Custom\\ModsWwise\\originals\\SFX\\vo_4TH_PRIEST__Here_to_lecture_us.wav"
                },
                new AudioFile {
                    Name="vo_4TH_PRIEST__Look_who_it_is",
                    Path="Custom\\ModsWwise\\originals\\SFX\\vo_4TH_PRIEST__Look_who_it_is.wav"
                },
            };
            CollectionAssert.AreEqual(expectedAudioFiles, project.AudioFiles);

            var expectedAudioObjects = new List<AudioObject> {
                new AudioObject {
                    Attenuation=0,
                    Guid="{074234A4-0367-471E-89CF-D199FD5477F1}",
                    Is2D=false,
                    IsLooping=false,
                    Name="mod_generic",
                    Source=null
                },
                new AudioObject {
                    Attenuation=0,
                    Guid="{43BA1D2D-5DBD-4B73-B506-03C416E7F0CF}",
                    Is2D=false,
                    IsLooping=false,
                    Name="generic1",
                    Source=new ExternalAudioSource {
                        Guid="{32A8900D-BD90-4F55-9568-463D4EF18545}",
                        Id=1377388561,
                        Path="..\\ModWavs\\generic\\generic1.wav"
                    }
                },
                new AudioObject {
                    Attenuation=0,
                    Guid="{F75B1E9A-7E0F-40B1-A6D3-901BF025B869}",
                    Is2D=false,
                    IsLooping=false,
                    Name="generic2",
                    Source=new ExternalAudioSource {
                        Guid="{B9572153-EAA0-4229-AB0E-6E86EA058916}",
                        Id=1377388562,
                        Path="..\\ModWavs\\generic\\generic2.wav"
                    }
                },
                new AudioObject {
                    Attenuation=0,
                    Guid="{35396071-50DA-45AE-9A2C-ACF5D36C2298}",
                    Is2D=false,
                    IsLooping=false,
                    Name="generic3",
                    Source=new ExternalAudioSource {
                        Guid="{AD29341A-3F85-4A79-BD14-BEDF0B022D4D}",
                        Id=1377388563,
                        Path="..\\ModWavs\\generic\\generic3.wav"
                    }
                },
                new AudioObject {
                    Attenuation=0,
                    Guid="{86ECA2AC-F9B1-45B7-A477-31F97C07A044}",
                    Is2D=true,
                    IsLooping=false,
                    Name="vo",
                    Source=null
                },
                new AudioObject {
                    Attenuation=25,
                    Guid="{4C5071DC-8262-4EE1-A9EA-BC99AA5FD19A}",
                    Is2D=false,
                    IsLooping=false,
                    Name="vo_4TH_PRIEST__Here_to_lecture_us",
                    Source=new EmbeddedAudioSource {
                        Duration=15.669F,
                        Id=1006448442,
                        Path="originals\\SFX\\vo_4TH_PRIEST__Here_to_lecture_us.wav",
                        TrimBegin=-0.001F,
                        TrimEnd=-0.001F
                    }
                },
                new AudioObject {
                    Attenuation=0,
                    Guid="{697CE0E6-4909-4E10-BD92-0F59E4C83D5C}",
                    Is2D=true,
                    IsLooping=false,
                    Name="vo_4TH_PRIEST__Look_who_it_is",
                    Source=new EmbeddedAudioSource {
                        Duration=10.4243956F,
                        Id=1045640917,
                        Path="originals\\SFX\\vo_4TH_PRIEST__Look_who_it_is.wav",
                        TrimBegin=1.15F,
                        TrimEnd=8.12345F
                    }
                },
            };
            CollectionAssert.AreEqual(expectedAudioObjects, project.AudioObjects);

            var expectedEventObjects = new List<EventObject> {
                new EventObject {
                    AudioObjects=new List<AudioObject> {
                        new AudioObject {
                            Attenuation=0,
                            Guid="{35396071-50DA-45AE-9A2C-ACF5D36C2298}",
                            Is2D=false,
                            IsLooping=false,
                            Name="generic3",
                            Source=new ExternalAudioSource {
                                Guid="{AD29341A-3F85-4A79-BD14-BEDF0B022D4D}",
                                Id=1377388563,
                                Path="..\\ModWavs\\generic\\generic3.wav"
                            }
                        }
                    },
                    Guid="{CF839B38-A76D-4554-924E-F3B3F6812075}",
                    Id=3332801300,
                    Name="Play_generic3",
                    PathParts=new List<string> { "Default Work Unit", "mod_events", "ghost_generic_mod" },
                    SoundbankId=3313201758,
                    TargetAudioObject=new AudioObject {
                        Attenuation=0,
                        Guid="{35396071-50DA-45AE-9A2C-ACF5D36C2298}",
                        Is2D=false,
                        IsLooping=false,
                        Name="generic3",
                        Source=new ExternalAudioSource {
                            Guid="{AD29341A-3F85-4A79-BD14-BEDF0B022D4D}",
                            Id=1377388563,
                            Path="..\\ModWavs\\generic\\generic3.wav"
                        }
                    },
                    TargetGuid="{35396071-50DA-45AE-9A2C-ACF5D36C2298}",
                    Type=EventType.Play
                },

                new EventObject {
                    AudioObjects=new List<AudioObject> {
                        new AudioObject {
                            Attenuation=0,
                            Guid="{43BA1D2D-5DBD-4B73-B506-03C416E7F0CF}",
                            Is2D=false,
                            IsLooping=false,
                            Name="generic1",
                            Source=new ExternalAudioSource {
                                Guid="{32A8900D-BD90-4F55-9568-463D4EF18545}",
                                Id=1377388561,
                                Path="..\\ModWavs\\generic\\generic1.wav"
                            }
                        }
                    },
                    Guid="{2B494D31-3D02-4C39-BEF6-CB9889E9EE32}",
                    Id=3332801302,
                    Name="Play_generic1",
                    PathParts=new List<string> { "Default Work Unit", "mod_events", "ghost_generic_mod" },
                    SoundbankId=3313201758,
                    TargetAudioObject=new AudioObject {
                        Attenuation=0,
                        Guid="{43BA1D2D-5DBD-4B73-B506-03C416E7F0CF}",
                        Is2D=false,
                        IsLooping=false,
                        Name="generic1",
                        Source=new ExternalAudioSource {
                            Guid="{32A8900D-BD90-4F55-9568-463D4EF18545}",
                            Id=1377388561,
                            Path="..\\ModWavs\\generic\\generic1.wav"
                        }
                    },
                    TargetGuid="{43BA1D2D-5DBD-4B73-B506-03C416E7F0CF}",
                    Type=EventType.Play
                },

                new EventObject {
                    AudioObjects=new List<AudioObject> {
                        new AudioObject {
                            Attenuation=0,
                            Guid="{F75B1E9A-7E0F-40B1-A6D3-901BF025B869}",
                            Is2D=false,
                            IsLooping=false,
                            Name="generic2",
                            Source=new ExternalAudioSource {
                                Guid="{B9572153-EAA0-4229-AB0E-6E86EA058916}",
                                Id=1377388562,
                                Path="..\\ModWavs\\generic\\generic2.wav"
                            }
                        }
                    },
                    Guid="{7DD32D72-1E1C-426C-9B6D-6E1BC2B3F52F}",
                    Id=3332801301,
                    Name="Play_generic2",
                    PathParts=new List<string> { "Default Work Unit", "mod_events", "ghost_generic_mod" },
                    SoundbankId=3313201758,
                    TargetAudioObject=new AudioObject {
                        Attenuation=0,
                        Guid="{F75B1E9A-7E0F-40B1-A6D3-901BF025B869}",
                        Is2D=false,
                        IsLooping=false,
                        Name="generic2",
                        Source=new ExternalAudioSource {
                            Guid="{B9572153-EAA0-4229-AB0E-6E86EA058916}",
                            Id=1377388562,
                            Path="..\\ModWavs\\generic\\generic2.wav"
                        }
                    },
                    TargetGuid="{F75B1E9A-7E0F-40B1-A6D3-901BF025B869}",
                    Type=EventType.Play
                },
                
                new EventObject {
                    AudioObjects=new List<AudioObject> {
                        new AudioObject {
                            Attenuation=0,
                            Guid="{697CE0E6-4909-4E10-BD92-0F59E4C83D5C}",
                            Is2D=true,
                            IsLooping=false,
                            Name="vo_4TH_PRIEST__Look_who_it_is",
                            Source=new EmbeddedAudioSource {
                                Duration=10.4243956F,
                                Id=1045640917,
                                Path="originals\\SFX\\vo_4TH_PRIEST__Look_who_it_is.wav",
                                TrimBegin=1.15F,
                                TrimEnd=8.12345F
                            }
                        }
                    },
                    Guid="{0B3B1632-67B9-4D14-844C-FE9496F2F1A1}",
                    Id=2344951565,
                    Name="Play_vo_4TH_PRIEST__Look_who_it_is",
                    PathParts=new List<string> { "Default Work Unit", "mod_events", "ghost_vo_mod" },
                    SoundbankId=3313201758,
                    TargetAudioObject=new AudioObject {
                        Attenuation=0,
                        Guid="{697CE0E6-4909-4E10-BD92-0F59E4C83D5C}",
                        Is2D=true,
                        IsLooping=false,
                        Name="vo_4TH_PRIEST__Look_who_it_is",
                        Source=new EmbeddedAudioSource {
                            Duration=10.4243956F,
                            Id=1045640917,
                            Path="originals\\SFX\\vo_4TH_PRIEST__Look_who_it_is.wav",
                            TrimBegin=1.15F,
                            TrimEnd=8.12345F
                        }
                    },
                    TargetGuid="{697CE0E6-4909-4E10-BD92-0F59E4C83D5C}",
                    Type=EventType.Play
                },
                
                new EventObject {
                    AudioObjects=new List<AudioObject> {
                        new AudioObject {
                            Attenuation=0,
                            Guid="{697CE0E6-4909-4E10-BD92-0F59E4C83D5C}",
                            Is2D=true,
                            IsLooping=false,
                            Name="vo_4TH_PRIEST__Look_who_it_is",
                            Source=new EmbeddedAudioSource {
                                Duration=10.4243956F,
                                Id=1045640917,
                                Path="originals\\SFX\\vo_4TH_PRIEST__Look_who_it_is.wav",
                                TrimBegin=1.15F,
                                TrimEnd=8.12345F
                            }
                        }
                    },
                    Guid="{937547C3-3AEC-44F1-BA63-E5C5296E0F7F}",
                    Id=484130019,
                    Name="Stop_vo_4TH_PRIEST__Look_who_it_is",
                    PathParts=new List<string> { "Default Work Unit", "mod_events", "ghost_vo_mod" },
                    SoundbankId=3313201758,
                    TargetAudioObject=new AudioObject {
                        Attenuation=0,
                        Guid="{697CE0E6-4909-4E10-BD92-0F59E4C83D5C}",
                        Is2D=true,
                        IsLooping=false,
                        Name="vo_4TH_PRIEST__Look_who_it_is",
                        Source=new EmbeddedAudioSource {
                            Duration=10.4243956F,
                            Id=1045640917,
                            Path="originals\\SFX\\vo_4TH_PRIEST__Look_who_it_is.wav",
                            TrimBegin=1.15F,
                            TrimEnd=8.12345F
                        }
                    },
                    TargetGuid="{697CE0E6-4909-4E10-BD92-0F59E4C83D5C}",
                    Type=EventType.Stop
                },
                
                new EventObject {
                    AudioObjects=new List<AudioObject> {
                        new AudioObject {
                            Attenuation=25,
                            Guid="{4C5071DC-8262-4EE1-A9EA-BC99AA5FD19A}",
                            Is2D=false,
                            IsLooping=false,
                            Name="vo_4TH_PRIEST__Here_to_lecture_us",
                            Source=new EmbeddedAudioSource {
                                Duration=15.669F,
                                Id=1006448442,
                                Path="originals\\SFX\\vo_4TH_PRIEST__Here_to_lecture_us.wav",
                                TrimBegin=-0.001F,
                                TrimEnd=-0.001F
                            }
                        }
                    },
                    Guid="{50056285-754A-437B-BC59-E30A874BE564}",
                    Id=2891876526,
                    Name="Play_vo_4TH_PRIEST__Here_to_lecture_us",
                    PathParts=new List<string> { "Default Work Unit", "mod_events", "ghost_vo_mod" },
                    SoundbankId=3313201758,
                    TargetAudioObject=new AudioObject {
                        Attenuation=25,
                        Guid="{4C5071DC-8262-4EE1-A9EA-BC99AA5FD19A}",
                        Is2D=false,
                        IsLooping=false,
                        Name="vo_4TH_PRIEST__Here_to_lecture_us",
                        Source=new EmbeddedAudioSource {
                            Duration=15.669F,
                            Id=1006448442,
                            Path="originals\\SFX\\vo_4TH_PRIEST__Here_to_lecture_us.wav",
                            TrimBegin=-0.001F,
                            TrimEnd=-0.001F
                        }
                    },
                    TargetGuid="{4C5071DC-8262-4EE1-A9EA-BC99AA5FD19A}",
                    Type=EventType.Play
                },
            };
            CollectionAssert.AreEqual(expectedEventObjects, project.EventObjects);

            var expectedSoundbankIds = new Dictionary<string, uint> {
                {"mods", 3313201758},
            };
            CollectionAssert.AreEqual(expectedSoundbankIds, project.SoundbanksInfo.SoundbankIds);

            var expectedEventIds = new Dictionary<string, uint> {
                {"{2B494D31-3D02-4C39-BEF6-CB9889E9EE32}", 3332801302},
                {"{7DD32D72-1E1C-426C-9B6D-6E1BC2B3F52F}", 3332801301},
                {"{CF839B38-A76D-4554-924E-F3B3F6812075}", 3332801300},
                {"{50056285-754A-437B-BC59-E30A874BE564}", 2891876526},
                {"{0B3B1632-67B9-4D14-844C-FE9496F2F1A1}", 2344951565},
                {"{937547C3-3AEC-44F1-BA63-E5C5296E0F7F}", 484130019},
            };
            CollectionAssert.AreEqual(expectedEventIds, project.SoundbanksInfo.EventIds);

            var expectedExternalSources = new Dictionary<string, ExternalSource> {
                {
                    "{32A8900D-BD90-4F55-9568-463D4EF18545}",
                    new ExternalSource {
                        Id = 1377388561,
                        Guid = "{32A8900D-BD90-4F55-9568-463D4EF18545}",
                        Name = "generic1"
                    }
                },
                {
                    "{B9572153-EAA0-4229-AB0E-6E86EA058916}",
                    new ExternalSource {
                        Id = 1377388562,
                        Guid = "{B9572153-EAA0-4229-AB0E-6E86EA058916}",
                        Name = "generic2"
                    }
                },
                {
                    "{AD29341A-3F85-4A79-BD14-BEDF0B022D4D}",
                    new ExternalSource {
                        Id = 1377388563,
                        Guid = "{AD29341A-3F85-4A79-BD14-BEDF0B022D4D}",
                        Name = "generic3"
                    }
                },
            };
            CollectionAssert.AreEqual(expectedExternalSources, project.SoundbanksInfo.ExternalSources);

            var expectedMediaFiles = new Dictionary<string, MediaFile> {
                {
                    "1006448442",
                    new MediaFile {
                        Duration=15.669F,
                        Id=1006448442,
                        Name="vo_4TH_PRIEST__Here_to_lecture_us.wav",
                        Path="SFX\\vo_4TH_PRIEST__Here_to_lecture_us_20F14839.wem"
                    }
                },
                {
                    "1045640917",
                    new MediaFile {
                        Duration=10.4243956F,
                        Id=1045640917,
                        Name="vo_4TH_PRIEST__Look_who_it_is.wav",
                        Path="SFX\\vo_4TH_PRIEST__Look_who_it_is_20F14839.wem"
                    }
                },
            };
            CollectionAssert.AreEqual(expectedMediaFiles, project.SoundbanksInfo.MediaFiles);

            var expectedPathParts = new List<string> {
                "Default Work Unit",
                "mod_events",
                "ghost_generic_mod",
                "ghost_vo_mod",
            };
            CollectionAssert.AreEqual(expectedPathParts, project.SoundbanksInfo.ObjectPathParts);
        }
    }
}
