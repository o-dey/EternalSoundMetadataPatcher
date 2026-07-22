using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EternalSoundMetadataPatcher.Test.TestTools.Assertion
{
    public static class ExceptionAssertion
    {
        public static T Throws<T>(Action action) where T : Exception
        {
            return Assert.ThrowsException<T>(action);
        }

        public static T ThrowsWithMessage<T>(string message, Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(message, ex.Message);

                return Assert.ThrowsException<T>(() => throw ex);
            }

            return Assert.ThrowsException<T>(() => null);
        }
    }
}
