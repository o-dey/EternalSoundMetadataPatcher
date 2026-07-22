using System;

namespace EternalSoundMetadataPatcher.Wwise.Structure.Audio
{
    public class AudioObject
    {
        public string Guid;
        public string Name;
        public float Attenuation;
        public bool Is2D;
        public bool IsLooping;
        public AudioSource Source;
    }
}
