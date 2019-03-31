using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var jsonData = @"{  'request': {    'Username': 'admin',    'Password': '123456',    'RememberMe': false  }    }";
            var obfuscatedData = JsonHelper.ObfuscateFieldValues(jsonData, ApplicationConstants.ObfuscatedActionArgumentFields);


        }
    }
}
