using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adoxio.Dynamics.Connect;
using System;
using System.Configuration;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.ServiceModel;
using System.Diagnostics;

namespace Adoxio.Dynamics.ConnectTests
{
    [TestClass()]
    public class CrmContextTests
    {
        
        [TestMethod()]
        public void CrmContext_DefaultTest()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;

            var context = new CrmContext();
            WhoAmITest(context);
        }

        [TestMethod()]
        public void CrmContext_StringTest()
        {
            var context = new CrmContext(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);
            WhoAmITest(context);
        }

        [TestMethod()]
        public void CrmContext_ObjectTest()
        {
            var settings = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);
            var context = new CrmContext(settings);
            WhoAmITest(context);
        }

        [TestMethod()]
        public void CrmContext_DefaultCreateTest()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;

            var context = CrmContext.Create();
            WhoAmITest(context);
        }

        [TestMethod()]
        public void CrmContext_StringCreateTest()
        {
            var context = CrmContext.Create(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);
            WhoAmITest(context);
        }

        [TestMethod()]
        public void CrmContext_ObjectCreateTest()
        {
            var settings = new S2SAppSettings(S2SProp.testClientId, S2SProp.testClientSecret, S2SProp.testResource, S2SProp.testTenantId);

            var context = CrmContext.Create(settings);
            WhoAmITest(context);
        }

        [TestMethod()]
        public void CrmContext_DefaultMultipleContext_Test()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;

            var context = new CrmContext();
            WhoAmITest(context);

            var context2 = new CrmContext();
            WhoAmITest(context2);

            var context3 = new CrmContext();
            WhoAmITest(context3);
        }

        [TestMethod()]
        public void CrmContext_DefaultMultipleContextNoClear_Test()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;
            
            var context = new CrmContext();
            var context2 = new CrmContext();
            var context3 = new CrmContext();

            try
            {
                WhoAmITest(context, false);
                WhoAmITest(context2, false);
                WhoAmITest(context3, false);
            }
            finally
            {
                context.Dispose();
                context.ResetAll();
                context2.Dispose();
                context2.ResetAll();
                context3.Dispose();
                context3.ResetAll();
            }
        }

        [TestMethod()]
        public void CrmContext_DefaultMultipleRequestsNoClear_Test()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;

            var context = new CrmContext();
            try
            {
                WhoAmITest(context, false);
                WhoAmITest(context, false);
                WhoAmITest(context, false);
            }
            finally
            {
                context.Dispose();
                context.ResetAll();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AdalServiceException))]
        public void CrmContext_Default_InvalidClientId_Throws()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = Guid.NewGuid().ToString();
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;

            var context = new CrmContext();
            WhoAmITest(context);
        }

        [TestMethod]
        [ExpectedException(typeof(AdalServiceException))]
        public void CrmContext_Default_InvalidClientSecret_Throws()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = "bullshit";
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;

            var context = new CrmContext();
            WhoAmITest(context);
        }

        [TestMethod]
        [ExpectedException(typeof(EndpointNotFoundException))]
        public void CrmContext_Default_InvalidResource_Throws()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = "https://thereisnowaythisexists.crm.dynamics.com";
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;

            var context = new CrmContext();
            WhoAmITest(context);
        }

        [TestMethod]
        [ExpectedException(typeof(AdalServiceException))]
        public void CrmContext_Default_InvalidTenantId_Throws()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = Guid.NewGuid().ToString();

            var context = new CrmContext();
            WhoAmITest(context);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrmContext_Default_NullClientId_Throws()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = null;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;

            var context = new CrmContext();
            WhoAmITest(context);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CrmContext_Default_NullClientSecret_Throws()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = null;
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;

            var context = new CrmContext();
            WhoAmITest(context);
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void CrmContext_Default_NullResource_Throws()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = null;
            ConfigurationManager.AppSettings["dyn:tenantId"] = S2SProp.testTenantId;

            var context = new CrmContext();
            WhoAmITest(context);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrmContext_Default_NullTenantId_Throws()
        {
            ConfigurationManager.AppSettings["dyn:ClientId"] = S2SProp.testClientId;
            ConfigurationManager.AppSettings["dyn:ClientSecret"] = S2SProp.testClientSecret;
            ConfigurationManager.AppSettings["dyn:Resource"] = S2SProp.testResource;
            ConfigurationManager.AppSettings["dyn:tenantId"] = null;

            var context = new CrmContext();
            WhoAmITest(context);
        }

        #region private test helpers

        private static void WhoAmITest(CrmContext context, bool clear = true)
        {
            try
            {
                var response = (WhoAmIResponse)context.WebProxyClient.Execute(new WhoAmIRequest());
                
                Assert.IsNotNull(response);
                Assert.IsNotNull(response.UserId);
                Trace.TraceInformation($"WhoAmI UserId: {response.UserId}");
                Assert.IsNotNull(response.BusinessUnitId);
                Trace.TraceInformation($"WhoAmI BusinessUnitId: {response.BusinessUnitId}");
                Assert.IsNotNull(response.OrganizationId);
                Trace.TraceInformation($"WhoAmI OrganizationId: {response.OrganizationId}");
            }
            finally
            {
                if (clear)
                {
                    context.Dispose();
                    context.ResetAll();
                }
            }            
        }

        #endregion
    }
}