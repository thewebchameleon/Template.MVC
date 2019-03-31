using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Template.Common.Helpers;
using Template.Infrastructure.Configuration;

namespace Template.Tests
{
    [TestClass]
    public class JsonHelperTestFixture
    {
        [TestMethod]
        public void Ensure_JsonHelper_Can_Obfuscate_Field_Values()
        {
            var fieldsToObfuscate = ApplicationConstants.ObfuscatedActionArgumentFields;
            var fieldValueToAssert = "123456"; // password

            var jsonData = @"{  'request': {    'Username': 'admin',    'Password': '123456'    }    }";
            var obfuscatedData = JsonHelper.ObfuscateFieldValues(jsonData, fieldsToObfuscate);

            Assert.IsTrue(jsonData.Contains(fieldValueToAssert));
            Assert.IsFalse(obfuscatedData.Contains(fieldValueToAssert));
        }

        [TestMethod]
        public void Ensure_JsonHelper_Can_Obfuscate_Field_Values_Without_Fields()
        {
            var fieldsToObfuscate = new List<string>() { "NonExistantFieldValue" };

            var jsonData = @"{  'request': {    'RandomId': '1',    'RandomName': 'Hamburger'    }    }";
            JsonHelper.ObfuscateFieldValues(jsonData, fieldsToObfuscate);
        }
    }
}
