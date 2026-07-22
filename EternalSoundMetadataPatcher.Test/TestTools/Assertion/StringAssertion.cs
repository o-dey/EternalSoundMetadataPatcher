using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EternalSoundMetadataPatcher.Test.TestTools.Assertion
{
    public static class StringAssertion
    {
        public static void AreEqualNormalized(string a, string b)
        {
            Assert.AreEqual(
                NormalizeEOL(a),
                NormalizeEOL(b)
            );
        }

        private static string NormalizeEOL(string text)
        {
            return text.Replace("\r\n", "\n").Replace("\r", "\n");
        }
    }
}
