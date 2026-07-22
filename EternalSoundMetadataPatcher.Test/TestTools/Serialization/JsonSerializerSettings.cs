using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EternalSoundMetadataPatcher.Test.TestTools.Serialization
{
    public class JsonSerializerSettings : Newtonsoft.Json.JsonSerializerSettings
    {
        internal const string DefaultNewlineCharacter = "\n";
        internal string _newlineCharacter;

        public string NewlineCharacter
        {
            get => _newlineCharacter ?? DefaultNewlineCharacter;
            set => _newlineCharacter = value;
        }
    }
}
