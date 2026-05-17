using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EternalSoundMetadataPatcher.Tests.TestTools.UnitTesting
{
    class ExpectedExceptionAndMessage : ExpectedExceptionBaseAttribute
    {
        private Type _expectedExceptionType;
        private string _expectedExceptionMessage;

        public ExpectedExceptionAndMessage(Type expectedExceptionType, string expectedExceptionMessage)
        {
            _expectedExceptionType = expectedExceptionType;
            _expectedExceptionMessage = expectedExceptionMessage;
        }

        protected override void Verify(Exception exception)
        {
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, _expectedExceptionType);
            Assert.AreEqual(_expectedExceptionMessage, exception.Message);
        }
    }
}
