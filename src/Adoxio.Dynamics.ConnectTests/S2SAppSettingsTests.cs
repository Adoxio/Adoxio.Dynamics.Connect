using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adoxio.Dynamics.Connect;

namespace Adoxio.Dynamics.ConnectTests
{
    [TestClass]
    public class S2SAppSettingsTests
    {
        [TestMethod]
        public void S2SAppSetting_Default_Test()
        {
            var setting = new S2SAppSettings()
            {
                ClientId = S2SProp.testClientId,
                ClientSecret = S2SProp.testClientSecret,
                Resource = S2SProp.testResource,
                TenantId = S2SProp.testTenantId
            };

            Assert.IsNotNull(setting);
        }

        [TestMethod]
        public void S2SAppSetting_PropConstructor_Test()
        {
            var setting = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);

            Assert.IsNotNull(setting);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void S2SAppSetting_PropConstructor_NullClientId_Throws()
        {
            var setting = new S2SAppSettings(null, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void S2SAppSetting_PropConstructor_NonGuidClientId_Throws()
        {
            var setting = new S2SAppSettings("asdf", S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void S2SAppSetting_PropConstructor_NullClientSecret_Throws()
        {
            var setting = new S2SAppSettings(S2SProp.testClientId, null, S2SProp.testResource, S2SProp.testTenantId);
            //setting.AssertAppSettings();
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void S2SAppSetting_PropConstructor_NullResource_Throws()
        {
            var setting = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, null, S2SProp.testTenantId);
            //setting.AssertAppSettings();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void S2SAppSetting_PropConstructor_NullTenantId_Throws()
        {
            var setting = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, null);
            //setting.AssertAppSettings();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void S2SAppSetting_PropConstructor_NonGuidTenantId_Throws()
        {
            var setting = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, "asdf");
            //setting.AssertAppSettings();
        }


        [TestMethod]
        public void S2SAppSetting_EqualsTrue_Test()
        {
            var setting = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);

            Assert.IsTrue(setting.Equals(setting));
        }

        [TestMethod]
        public void S2SAppSetting_EqualsFalseClientId_Test()
        {
            var setting = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);
            var setting2 = new S2SAppSettings(Guid.NewGuid().ToString(), S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);

            Assert.IsFalse(setting.Equals(setting2));
        }

        [TestMethod]
        public void S2SAppSetting_EqualsFalseClientSecret_Test()
        {
            var setting = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);
            var setting2 = new S2SAppSettings(Guid.NewGuid().ToString(), "bullshit", S2SProp.testResource, S2SProp.testTenantId);

            Assert.IsFalse(setting.Equals(setting2));
        }

        [TestMethod]
        public void S2SAppSetting_EqualsFalseResource_Test()
        {
            var setting = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);
            var setting2 = new S2SAppSettings(Guid.NewGuid().ToString(), S2SProp.testClientSecret, "https://thereisnowaythisexists.crm.dynamics.com", S2SProp.testTenantId);

            Assert.IsFalse(setting.Equals(setting2));
        }

        [TestMethod]
        public void S2SAppSetting_EqualsFalseTenantId_Test()
        {
            var setting = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);
            var setting2 = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, Guid.NewGuid().ToString());

            Assert.IsFalse(setting.Equals(setting2));
        }
    }
}
